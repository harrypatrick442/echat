using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Shutdown;
using WebSocketSharp.Server;
using FilesRelayCore.TransferServers;
using Microsoft.FeatureManagement;
using FilesRelayCore.Controllers;
using WebAPI.Controllers;
using Core.Enums;
using MaintenanceCore;
using WebAbstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using EChatEndpoints.WebsocketServers;
using Core.MemoryManagement;
using Core.LoadBalancing;
using MultimediaServerCore;
using MultimediaType = MultimediaServerCore.MultimediaType;
using MultimediaServer;
using Authentication;
using Users;
namespace EChatGeneric
{
    public class EChatStartup
    {
        private bool _IsDebug =
#if DEBUG
            true
#else
            false
#endif
            ;
        public IConfiguration Configuration { get; }

        private static readonly CredentialsSetup CREDENTIALS_SETUP = new CredentialsSetup(
            guestEnabled: EChat.Constants.EChatCredentialsSetup.GUEST_ENABLED,
            emailRequiredToRegister: EChat.Constants.EChatCredentialsSetup.EMAIL_REQUIRED_TO_REGISTER,
            phoneRequiredToRegister: EChat.Constants.EChatCredentialsSetup.PHONE_REQUIRED_TO_REGISTER,
            usernameRequiredToRegister: EChat.Constants.EChatCredentialsSetup.USERNAME_REQUIRED_TO_REGISTER,
            passwordMinLength: EChat.Constants.EChatCredentialsSetup.PASSWORD_MIN_LENGTH,
            passwordMaxLength: EChat.Constants.EChatCredentialsSetup.PASSWORD_MAX_LENGTH,
            usernameMinLength: EChat.Constants.EChatCredentialsSetup.USERNAME_MIN_LENGTH,
            usernameMaxLength: EChat.Constants.EChatCredentialsSetup.USERNAME_MAX_LENGTH,
            usernamesUnique: EChat.Constants.EChatCredentialsSetup.USERNAME_UNIQUE,
            emailPasswordLogInEnabled: EChat.Constants.EChatCredentialsSetup.EMAIL_PASSWORD_LOG_IN_ENABLED,
            phonePasswordLogInEnabled: EChat.Constants.EChatCredentialsSetup.PHONE_PASSWORD_LOG_IN_ENABLED,
            emailOnlyLogInEnabled: EChat.Constants.EChatCredentialsSetup.EMAIL_ONLY_LOG_IN_ENABLED);
        public EChatStartup(IConfiguration configuration)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Configuration = configuration;
            Logs.Add(Logging.LogServerClient.Initialize(
                PlatformHelper.GetPlatform(), Core.Enums.Project.TransferServerBackend, Nodes.Nodes.Instance.MyId));
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureManagement();
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            CorsSetup.EChatServer(services);
#if DEBUG
            CorsSetup.MultimediaServer(services);
            CorsSetup.Authentication(services);
#endif
            services.AddControllers()
                .AddApplicationPart(typeof(MultimediaServerController).Assembly)
                .AddApplicationPart(typeof(MachineMetricsController).Assembly)
                .AddApplicationPart(typeof(NodesController).Assembly)
                .AddApplicationPart(typeof(AuthenticationController).Assembly);
            services.AddHttpContextAccessor();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            try
            {

                applicationLifetime.ApplicationStopping.Register(
                    () => ShutdownManager.Instance.Shutdown());
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                app.UseRouting();

                app.UseCors(builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://10.0.2.2:3000") //Source
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST")
                        .AllowCredentials();
                });

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                WebSocketServer webSocketServer = WebSocketStartup.Run(
                    Configuration.GetValue<string>);
                MemoryManager.Initialize(GlobalConstants.Sizes.GetBytesMemoryAllowedForNode(
                    Nodes.Nodes.Instance.MyId));
                InterserverComs.Initializer.Initialize(webSocketServer);
                KeyValuePairDatabases.Initializer.Initialize();
                //ProcessorMetricsSource.Initialize();
                MachineMetricsMesh.Initialize(loggingEnabled:false);

                NodeAssignedIdRanges.Initializer.Initialize(_IsDebug);
                UserRoutedMessages.Initializer.Initialize();
                UserRouting.Initializer.Initialize(debugLoggingEnabled:false);
                MaintenanceInitializer.Initialize(controlsMaintenance: false,
                    receivesMaintenance: true, null);
                webSocketServer.AddWebSocketService<EChatRoomWebsocketServer>(
                    GlobalConstants.Endpoints.ECHAT_ROOM_WEBSOCKET);
                webSocketServer.AddWebSocketService<EChatUserWebsocketServer>(
                    GlobalConstants.Endpoints.ECHAT_USER_WEBSOCKET);
                webSocketServer.Start();
                EChatWebSocketLoadBroadcaster.Initialize();
                Sessions.Initializer.Initialize();
                Users.Initializer.Initialize();
                Authentication.Initializer.Initialize(_IsDebug,
                    UsersMesh.Instance, CREDENTIALS_SETUP,
                    EChatEmailing.EChatEmailEmailer.Instance
                );
                Location.Initializer.Initialize();
                UserLocation.Initializer.Initialize();
                UserIgnore.Initializer.Initialize();
                HashTags.Initializer.Initialize();
                Flagging.Initializer.Initialize(isFlaggingClient:true);
                Chat.Initializer.Initialize(_IsDebug);
                NotificationsCore.Initializer.Initialize();
                ReceivingLoadBalancer.Initialize();
                if (_IsDebug)
                {
                    MultimediaServerCore.Initializer.InitializeServer(new MultimediaServerSetup(
                            new MultimediaTypeSetup(MultimediaType.ConversationPicture,
                                ".webp"),
                            new MultimediaTypeSetup(MultimediaType.MessagePicture,
                                ".webp"),
                            new MultimediaTypeSetup(MultimediaType.MessageVideo,
                                ".mp4", ".avi", ".wmv", ".flv", ".mov", ".webm"),
                            new MultimediaTypeSetup(MultimediaType.ProfilePicture,
                                ".webp"),
                            new MultimediaTypeSetup(MultimediaType.CustomEmoticon,
                                ".webp")
                        ));
                }
                else
                {
                    MultimediaServerCore.Initializer.InitializeClient();
                }
                UserMultimediaCore.Initializer.Initialize();
                Firewall.Initialize().OpenPortsUntilShutdown(GlobalConstants.Ports.Value);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                ShutdownManager.Instance.Shutdown();
            }
        }
    }
}
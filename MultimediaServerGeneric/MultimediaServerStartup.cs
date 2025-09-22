using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Shutdown;
using WebSocketSharp.Server;
using FilesRelayCore;
using FilesRelayCore.TransferServers;
using Microsoft.FeatureManagement;
using InterserverComs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Core.MemoryManagement;
using FilesRelayCore.Controllers;
using WebAPI.Controllers;
using Core.LoadBalancing;
using Core.Enums;
using WebAbstract;
using MultimediaServerCore;
using MaintenanceCore;
using Logging_ClientFriendly;

namespace MultimediaServer
{
    public class MultimediaServerStartup
    {
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        
        public MultimediaServerStartup(IConfiguration configuration)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Configuration = configuration;
            Logs.Add(Logging.LogServerClient.Initialize(
                PlatformHelper.GetPlatform(), Core.Enums.Project.MutimediaServerBackend, Nodes.Nodes.Instance.MyId));
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
            CorsSetup.MultimediaServer(services);
            services.AddControllers()
                .AddApplicationPart(typeof(MultimediaServerController).Assembly)
                .AddApplicationPart(typeof(MachineMetricsController).Assembly)
                .AddApplicationPart(typeof(NodesController).Assembly);
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                    builder.WithOrigins("http://localhost:3000") //Source
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST")
                        .AllowCredentials();
                });

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                WebSocketServer webSocketServer = WebSocketStartup.Run(Configuration.GetValue<string>);
                MemoryManager.Initialize(GlobalConstants.Sizes.GetBytesMemoryAllowedForNode(
                    Nodes.Nodes.Instance.MyId));
                InterserverPort.Initialize(webSocketServer, CertificateManagement.Constants.TLS.FULL_CHAIN_PATH);
                InterserverInverseTicketedSender.Initialize();
                InterserverTicketedSender.Initialize();
                ProcessorMetricsSource.Initialize();
                LoadFactorsSource.Initialize(new MultimediaServerLoadFactorSource());
                MachineMetricsMesh.Initialize(loggingEnabled:false);
                MaintenanceInitializer.Initialize(controlsMaintenance: false,
                    receivesMaintenance: true, null);
                //MultimediaServerStatisticsFileLogger.Initialize();
                ReceivingLoadBalancer.Initialize();
                Flagging.Initializer.Initialize(isFlaggingClient: false);
                MultimediaServerCore.Initializer.InitializeServer(new MultimediaServerSetup(
                        new MultimediaTypeSetup(MultimediaType.ConversationPicture,
                            ".png", ".jpg", ".webp"),
                        new MultimediaTypeSetup(MultimediaType.MessagePicture,
                            ".png", ".jpg", ".webp"),
                        new MultimediaTypeSetup(MultimediaType.MessageVideo,
                            ".mp4", ".avi", ".wmv", ".flv", ".mov"),
                        new MultimediaTypeSetup(MultimediaType.ProfilePicture,
                            ".png", ".jpg", ".webp"),
                        new MultimediaTypeSetup(MultimediaType.CustomEmoticon,
                            ".png", ".jpg", ".webp")
                    ));
                webSocketServer.Start();
                Firewall.Initialize().OpenPortsUntilShutdown(GlobalConstants.Ports.Value);
                Logs.Default.Info("opening ports");
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}
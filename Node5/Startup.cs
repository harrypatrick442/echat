using FilesRelayCore.WebsocketServers;
using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Shutdown;
using WebSocketSharp.Server;
using Microsoft.FeatureManagement;
using InterserverComs;
using LogServer.Controllers;
using Authentication;
using MaintenanceControllers;
using MaintenanceCore;
using LogServerCore.DAL;
using NodeAssignedIdRanges;
using WebAbstract;
using WebAbstract.Csontrollers;
using WebAbstract.Controllers;
using WebAbstract.Extensions;

namespace LogServer
{
    public class Startup
    {
        private bool _IsDebug =
#if DEBUG
            true
#else
            false
#endif
            ;
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        
        public Startup(IConfiguration configuration)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Configuration = configuration;
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
            });
            //CorsSetup.ClientToClientFilesRelay(services);
            CorsSetup.LogServer(services);
            services.AddControllers()
                .AddApplicationPart(typeof(NodeAssignedIdRangesController).Assembly)
                .AddApplicationPart(typeof(LoggerController).Assembly)
                .AddApplicationPart(typeof(LogViewerController).Assembly)
                .AddApplicationPart(typeof(LogServerIndexController).Assembly)
                .AddApplicationPart(typeof(MachineMetricsController).Assembly)
                .AddApplicationPart(typeof(NodesController).Assembly)
                .AddApplicationPart(typeof(MaintenanceClientController).Assembly);
                //.AddApplicationPart(typeof(FilesRelayCore.Controllers.MachineMetricsController).Assembly);
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
#if RELEASE
                app.MapSubdomain("log", a => {
                    a.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(
                            EnvironmentVariables.Paths.Client_LogViewer),
                        RequestPath = new PathString(""),
                        //ContentTypeProvider = provider
                    });
                });
                app.MapSubdomain("maintenance", a => {
                    a.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(
                            EnvironmentVariables.Paths.Client_MaintenanceClient),
                        RequestPath = new PathString(""),
                        //ContentTypeProvider = provider
                    });
                });
#else
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(
                        EnvironmentVariables.Paths.Client_LogViewer),
                    RequestPath = new PathString(""),
                    //ContentTypeProvider = provider
                });
#endif
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                WebSocketServer webSocketServer = WebSocketStartup.Run(Configuration.GetValue<string>);
                webSocketServer.AddWebSocketService<MaintenanceClientWebsocketServer>(
                    Configurations.Endpoints.MAINTENANCE_CLIENT_WEBSOCKET);
                Core.MemoryManagement.MemoryManager.Initialize(1000000000);
                InterserverInverseTicketedSender.Initialize();
                InterserverTicketedSender.Initialize();
                InterserverPort.Initialize(webSocketServer, CertificateManagement.Constants.TLS.FULL_CHAIN_PATH);
                DalLogs.Initialize();
                //AppendedKeyValuePairDatabaseIncomingMessagesHandler.Initialize();
                AuthenticationAttemptByIPFrequencyManager.Initialize();
                ScheduledMaintenanceMesh.Initialize();
                Logger.Initialize();
                NodeAssignedIdRanges.Initializer.Initialize(true);
                webSocketServer.Start();
                Firewall.Initialize().OpenPortsUntilShutdown(Configurations.Ports.Value);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                throw new Exception("Failed to start server", ex);
            }
        }
    }
}
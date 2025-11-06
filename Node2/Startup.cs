using FilesRelayCore.TURN;
using FilesRelayCore.WebsocketServers;
using Core.ClientEndpoints;
using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Snippets.Shutdown;
using SnippetsCore.Assets;
using SnippetsCore.Runtime;
using WebSocketSharp.Server;
using Microsoft.FeatureManagement;
using FilesRelayCore;
using FilesRelayCore.TransferServers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using InterserverComs;
using FilesRelay;
using Logger = LogServer.Logger;
using KeyValuePairDatabases.Appended;
using SnippetsCore.MemoryManagement;
using FilesRelay.LoadBalancing;
using FilesRelayCore.Controllers;
using WebAPI.Controllers;
using Core.LoadBalancing;
using Core.Enums;
using FilesRelayControllers;
using TransferServerControllers;
using LogServer.Controllers;
using MaintenanceCore;
using FilesRelayCore.Endpoints;
using MaintenanceControllers;
using Authentication;
using WebAbstract.Extensions;
namespace FileServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        
        public Startup(IConfiguration configuration)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Configuration = configuration;
            ShutdownManager.Initialize(Environment.Exit, () => Logs.Default);
            Logs.Initialize(Paths.LogFilePathSnippets,
#if RELEASE
               LogServerClient.Initialize(
                PlatformHelper.GetPlatform(), Core.Enums.Project.FileServer, NodeConfiguration.Instance.Id)
#else
     null
#endif
     );
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
            CorsSetup.ClientToClientFilesRelay(services);
            services.AddControllers()
                .AddApplicationPart(typeof(FileServerController).Assembly)
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

                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(EnvironmentVariables.Paths.Client),
                    RequestPath = new PathString(""),
                    //ContentTypeProvider = provider
                });
                app.MapSubdomain("maintenance", a => {
                    a.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(
                            EnvironmentVariables.Paths.MaintenanceClient),
                        RequestPath = new PathString(""),
                        //ContentTypeProvider = provider
                    });
                });
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                WebSocketServer webSocketServer = WebSocketStartup.Run(Configuration.GetValue<string>);
                Nodes.Nodes nodes = Nodes.Nodes.Initialize();
                InterserverPort.Initialize(nodes, webSocketServer);
                InterserverInverseTicketedSender.Initialize();
                InterserverTicketedSender.Initialize();
                ProcessorMetricsSource.Initialize();
                MachineMetricsMesh.Initialize();
                Logger.Initialize();
#if DEBUG
                Logs.Add(LogServerClientDebug.Initialize(
                 PlatformHelper.GetPlatform(), Core.Enums.Project.FilesRelayBackend, NodeConfiguration.Instance.Id));
#endif
                webSocketServer.Start();
                Firewall.Initialize().OpenPortsUntilShutdown(Constants.Ports.Value);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                throw new Exception("Failed to start server", ex);
            }
        }
    }
}
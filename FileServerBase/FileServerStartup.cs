using Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Shutdown;
using WebSocketSharp.Server;
using Microsoft.FeatureManagement;
using InterserverComs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Core.MemoryManagement;
using Core.Enums;
using WebAbstract;
using MaintenanceCore;
using Statistics;
using WebAbstract.Csontrollers;
using WebAbstract.MachineMetricsMesh;
using WebAbstract.Controllers;
using WebAbstract.LoadBalancing;
namespace FileServerBase
{
    public class FileServerStartup
    {
        public IConfiguration Configuration { get; }
        public FileServerStartup(IConfiguration configuration)
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
            /*services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            CorsSetup.FileServerCors(services);*/
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
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                WebSocketServer webSocketServer = WebSocketStartup.Run(Configuration.GetValue<string>);
                InterserverPort.Initialize(webSocketServer, CertificateManagement.Constants.TLS.FULL_CHAIN_PATH);
                InterserverInverseTicketedSender.Initialize();
                InterserverTicketedSender.Initialize();
                ProcessorMetricsSource.Initialize();
                //LoadFactorsSource.Initialize(new TransferServerLoadFactorSource());
                MachineMetricsMesh.Initialize(loggingEnabled:false);
                ReceivingLoadBalancer.Initialize();
                MaintenanceInitializer.Initialize(controlsMaintenance: false,
                    receivesMaintenance: true, null);
                FileServerStatisticsFileLogger.Initialize();
                webSocketServer.Start();
                Firewall.Initialize().OpenPortsUntilShutdown(GlobalConstants.Ports.Value);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}
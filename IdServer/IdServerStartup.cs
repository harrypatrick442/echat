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
using Core.Enums;
using WebAbstract;
using System;
using Logging_ClientFriendly;

namespace IdServer
{
    public class IdServerStartup
    {
        public IConfiguration Configuration { get; }
        public IdServerStartup(IConfiguration configuration)
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
            CorsSetup.IdServerCors(services);
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
                NodeAssignedIdRanges.Initializer.Initialize(true);
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
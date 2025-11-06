using CertificateManagement;
using Logging;
using Shutdown;
using MultimediaServer;
using Core.FileSystem;
using Dependencies;
using Nodes;
using Configurations;

public class Program
{
    public const int NODE_ID = Configurations.Nodes.NODE_3_MS;
    public static void Main(string[] args)
    {
        Initializer.Initialize();
        Logs.Initialize(LogFilePathDefault.Value);
        AllStringDependenciesCrossProjects.Initialize();
        ShutdownManager.Initialize(Environment.Exit, () => Logs.Default);
        TLSCertificateManager.DoYourThing(NODE_ID);
        string nodesJson;
#if DEBUG
        nodesJson = GeneratedNodesJSON.Development;
#else
            nodesJson = GeneratedNodesJSON.Live;
#endif
        Nodes.Nodes.Initialize(NODE_ID, nodesJson);
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configBuilder) =>
            {
                configBuilder.SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<MultimediaServerStartup>();
            });

}
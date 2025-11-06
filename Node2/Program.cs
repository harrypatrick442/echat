
using CertificateManagement;
using Logging;
using Shutdown;
using Core.FileSystem;
using FileServer;
using Nodes;
using FileServerBase;
using Configurations;
public class Program
{
    public const int NODE_ID = Configurations.Nodes.FILE_SERVER_1;
    public static void Main(string[] args)
    {
        Initializer.Initialize();
        Logs.Initialize(LogFilePathDefault.Value);
        Dependencies.Initialize();
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
            webBuilder.UseStartup<FileServerStartup>();
        });
}
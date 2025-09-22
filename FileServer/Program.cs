
using CertificateManagement;
using Logging;
using Shutdown;
using FileServerBase;
using Core.FileSystem;
using FileServer;
using Nodes;
public class Program
{
    public const int NODE_ID = GlobalConstants.Nodes.FILE_SERVER_1;
    public static void Main(string[] args)
    {
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
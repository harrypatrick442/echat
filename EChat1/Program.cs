using Shutdown;
using EChatGeneric;
using Logging;
using Core.FileSystem;
using Dependencies;
using CertificateManagement;
using Nodes;
using Configurations;

public class Program
{
    public const int NODE_ID =
#if DEBUG
        Configurations.Nodes.ECHAT_DEBUG;
#else 
        Configurations.Nodes.ECHAT_1;
#endif
    public static void Main(string[] args)
    {
        Initializer.Initialize();
        Logs.Initialize(LogFilePathDefault.Value);
        AllStringDependenciesCrossProjects.Initialize();
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        ShutdownManager.Initialize(Environment.Exit, () => Logs.Default);
#if !DEBUG
        TLSCertificateManager.DoYourThing(NODE_ID);
#endif
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
                webBuilder.UseStartup<EChatStartup>();
            });

}
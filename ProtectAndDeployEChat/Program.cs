using Renci.SshNet;
using SSH;
using System.Diagnostics;
using System.Text;
using Core.Processes;
using RemoteAssistantCore;
using Setup.Git;
using System.Xml;
using Core;
using System.Reflection;
using System.Xml.Linq;
using Core.FileSystem;
using System.Security;
using Renci.SshNet.Messages;
public class Program
{
    private const string DOTNET_REACTOR_PATH = "C:\\Program Files (x86)\\Eziriz\\.NET Reactor\\dotNET_Reactor.Console.exe";
    public static void Main(string[] args)
    {
        Configurations.Initializer.Initialize();
        bool doBackend = AskYesNo("BuildDotNet and deploy backend?", true);
        bool doClient = AskYesNo("BuildDotNet and deploy client?", true);
        bool doCleanupServerLogs = AskYesNo("Cleanup server logs?", true);
        bool doRetrySslCertificateIfFailed = AskYesNo("Retry SSL certificate if failed?", true);
        bool doRestartServersAfter = AskYesNo("Restart servers after?", true);
        bool doInstallFfmpeg = AskYesNo("Install ffmpeg?", false);
        bool createFreshDb = AskToType("Create fresh db?", "Create fresh db");
        string reposDirectory = Assembly.GetEntryAssembly()!.Location;
        while (Path.GetFileName(reposDirectory).ToLower() != "repos")
        {
            reposDirectory = Directory.GetParent(reposDirectory)!.FullName;
        }
        string echatDirectory = Path.Combine(reposDirectory, "echat");
        if (doBackend)
        {
            string echatSolutionFilePath = Path.Combine(echatDirectory, "EChat.sln");
            BuildDotNet(echatSolutionFilePath);
            Func<string, string> protect = Create_Protect(DOTNET_REACTOR_PATH);
            // protect("C:\\repos\\snippets\\DotNetReactor\\FileServer.nrproj");
            // protect("C:\\repos\\snippets\\DotNetReactor\\EChat1.nrproj");
            // protect("C:\\repos\\snippets\\DotNetReactor\\MultimediaServer1.nrproj");
            // protect("C:\\repos\\snippets\\DotNetReactor\\LogServer.nrproj");

            string node1SecureDirectory = protect(Path.Combine(echatDirectory, "DotNetReactor\\Node1.nrproj"));
            string node2SecureDirectory = protect(Path.Combine(echatDirectory, "DotNetReactor\\Node2.nrproj"));
            string node3SecureDirectory = protect(Path.Combine(echatDirectory, "DotNetReactor\\Node3.nrproj"));
            string node4SecureDirectory = protect(Path.Combine(echatDirectory, "DotNetReactor\\Node5.nrproj"));

            //PuttyCommands.AgentCopyProjetToServer("fileserver", "C:\\repos\\snippets\\Build\\FileServer\\Release\\net7.0\\FileServer_Secure", Configurations.Nodes.FILE_SERVER_1);
            //PuttyCommands.AgentCopyProjetToServer("echat1", "C:\\repos\\snippets\\Build\\EChat1\\Release\\net7.0\\EChat1_Secure", Configurations.Nodes.ECHAT_1);
            // PuttyCommands.AgentCopyProjetToServer("multimediaserver1", "C:\\repos\\snippets\\Build\\MultimediaServer1\\Release\\net7.0\\MultimediaServer1_Secure", Configurations.Nodes.MULTIMEDIA_SERVER_1);
            //PuttyCommands.AgentCopyProjetToServer("logserver", "C:\\repos\\snippets\\Build\\LogServer\\Release\\net7.0\\LogServer_Secure", Configurations.Nodes.LOG_SERVER_1);

            PuttyCommands.AgentCopyProjectToServer("/var/node2", node2SecureDirectory, Configurations.Nodes.NODE_2_FS);
            PuttyCommands.AgentCopyProjectToServer("/var/node1", node1SecureDirectory, Configurations.Nodes.NODE_1_WS);
            PuttyCommands.AgentCopyProjectToServer("/var/node3", node3SecureDirectory, Configurations.Nodes.NODE_3_MS);
            PuttyCommands.AgentCopyProjectToServer("/var/node5", node4SecureDirectory, Configurations.Nodes.NODE_5);

        }
        if (doClient)
        {
            string echatClientPath = Path.Combine(reposDirectory, "echat-client");
            NPMHelper.RunScript("build_echat", echatClientPath);
            CreateDirectoryOnServer("/var/client/dev.e-chat.live", Configurations.Nodes.FILE_SERVER_1);
            PuttyCommands.AgentCopyProjectToServer("/var/client/dev.e-chat.live", Path.Combine(echatClientPath, "build_echat"), Configurations.Nodes.FILE_SERVER_1);
        }
        List<string> commands = new List<string> { };
        if (doCleanupServerLogs)
        {
            commands.Add("rm -rf /var/log/syslog");
            commands.Add("rm -rf /var/log/snippets.log");
        }
        if (doRetrySslCertificateIfFailed)
        {
            commands.Add("rm -rf /db/DontTryCertifyAgain.json");
        }
        if (doRestartServersAfter)
        {
            commands.Add("sudo shutdown -r");
        }
        if (doInstallFfmpeg)
        {
            Console.WriteLine(RunUsingSsh(Configurations.Nodes.MULTIMEDIA_SERVER_1,
                new string[] {
                        "sudo apt update"
                }
            )[0]);
            Console.WriteLine(RunUsingSsh(Configurations.Nodes.MULTIMEDIA_SERVER_1,
                new string[] {
                        "sudo apt --yes --force-yes install ffmpeg"
                }
            )[0]);
            Console.WriteLine(RunUsingSsh(Configurations.Nodes.MULTIMEDIA_SERVER_1,
                new string[] {
                        "ffmpeg -version"
                }
            )[0]);
        }
        if (createFreshDb) {
            StopNode(Configurations.Nodes.NODE_1_WS, "node1");
            CreateIdFilesAndCopyToServer(Configurations.Nodes.NODE_1_WS, "/db/sessionIds/", "sessionId", 1);
            CreateIdFilesAndCopyToServer(Configurations.Nodes.NODE_1_WS, "/db/requestUniqueIdentifier/", "currentId", 3);
            CreateIdFilesAndCopyToServer(Configurations.Nodes.NODE_1_WS, "/db/userIdSourceDirectory/", "currentId", 3);
            CreateIdFilesAndCopyToServer(Configurations.Nodes.NODE_1_WS, "/db/conversationIdSourceDirectory/", "currentId", 3);
            CreateIdFilesAndCopyToServer(Configurations.Nodes.NODE_1_WS, "/db/messageIdSource/", "currentId", 3);
            CreateIdFilesAndCopyToServer(Configurations.Nodes.NODE_1_WS, "/db/mentionIdSource/", "currentId", 3);
        }
        if (commands.Count > 0)
        {
            foreach (int nodeId in new int[] {
                Configurations.Nodes.NODE_1_WS ,
            //      Constants.Nodes.FILES_RELAY_2 ,
                Configurations.Nodes.NODE_2_FS,
                Configurations.Nodes.NODE_3_MS,
                Configurations.Nodes.NODE_5
            })
            {
                Console.WriteLine($"Now running commands on node {nodeId}");
                string[] results = RunUsingSsh(nodeId,
                    commands.ToArray()
                );
                foreach (string result in results)
                    Console.WriteLine(result);
            }
        }
    }
    private static void CreateIdFilesAndCopyToServer(int nodeId, string dbDirectoryOnServer, string fileNamePrefix, int nCopies) {
        using (TemporaryDirectory temporaryDirectory = new TemporaryDirectory()) {
            for (int nCopy = 0; nCopy < nCopies; nCopy++){
                string fileName = $"{fileNamePrefix}_{nCopy}.json";
                string filePath = Path.Combine(temporaryDirectory.AbsolutePath, fileName);
                File.WriteAllText(filePath, "0");
            }
            DeleteDirectoryOnServer(dbDirectoryOnServer, nodeId);
            CreateDirectoryOnServer(dbDirectoryOnServer, nodeId);
            PuttyCommands.AgentCopyProjectToServer(dbDirectoryOnServer, temporaryDirectory.AbsolutePath, nodeId);
        }
    }
    private static void StopNode(int nodeId, string serviceName)
    {

        RunUsingSsh(nodeId,
            $"systemctl stop {serviceName}"
        );
    }
    private static void DeleteDirectoryOnServer(string path, int nodeId) {

        string[] results = RunUsingSsh(nodeId,
            $"rm -rf {path}"
        );
    }
    private static void CreateDirectoryOnServer(string path, int nodeId) {

        string[] results = RunUsingSsh(nodeId,
            $"sudo mkdir -p {path}"
        );
    }
    private static string[] RunUsingSsh(int nodeId, params string[] commands)

    {

        var user = "root";
        var host = $"{Configurations.Nodes.Instance.GetIpOrDomainForNode(nodeId)}";
        byte[] openSshKey = Keys.Keys.GetOpenSSHs(nodeId);
        var pkRsa = new PrivateKeyFile(new MemoryStream(openSshKey));

        RsaSha256.ConvertToKeyWithSha256Signature(pkRsa);

        using (var authenticationMethodRsa = new PrivateKeyAuthenticationMethod(user, pkRsa))
        {
            var conn = new ConnectionInfo(host, user, authenticationMethodRsa);
            RsaSha256.SetupConnection(conn); // adds rsa-sha2-256

            using (var sshClient = new SshClient(conn))
            {
                sshClient.Connect();
                List<string> results = new List<string>();
                foreach (string command in commands)
                {
                    string result = sshClient.CreateCommand(command).Execute();
                    results.Add(result);
                }
                return results.ToArray();
            }
        }
    }

    private static Func<string, string> Create_Protect(string dotnetReactorPath)
    {
        return (nrprojPath) =>
        {
            UpdateNrprojFile(nrprojPath);
            StringBuilder sb = new StringBuilder();
            try
            {
                using (var handle = ProcessRunHelper.RunAsynchronously(dotnetReactorPath, null, $"-project \"{nrprojPath}\"",
                    (str) => sb.Append(str),
                    (str) => sb.Append(str)))
                {
                    if (handle.Wait() != 0)
                        throw new Exception($"Failed to protect \"{nrprojPath}\" with error \"{sb.ToString()}\"");
                }
            }
            finally
            {
                string output = sb.ToString();
                Console.WriteLine(output);
            }
            XDocument doc = XDocument.Load(nrprojPath);
            var mainAssembly = doc.Root?.Element("Main_Assembly")?.Value;
            string secureDirectory = Path.Combine(Path.GetDirectoryName(mainAssembly)!, $"{Path.GetFileNameWithoutExtension(nrprojPath)}_Secure");
            return secureDirectory;

            /*
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = $"\"{dotnetReactorPath}\"",
                Arguments = $"-project \"{nrprojPath}\"",
                UseShellExecute = true,
                RedirectStandardError = true,
                ErrorDialogParentHandle += (e, o)=>{ }
            };
            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            */
        };
    }
    private static void UpdateNrprojFile(string nrprojPath)
    {
        string backupFileName = Path.Combine(Path.GetDirectoryName(nrprojPath), Path.GetFileNameWithoutExtension(nrprojPath) + "_backup.nrproj");
        File.Copy(nrprojPath, backupFileName, true);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(File.ReadAllText(nrprojPath));
        XmlElement root = doc.DocumentElement;
        var mainAssemblyElements = root.GetElementsByTagName("Main_Assembly");
        if (mainAssemblyElements.Count < 1)
            throw new Exception($"Failed to find element \"Main_Assembly_EnforceRelativePath\" in .nrproj file \"{nrprojPath}\")");
        XmlElement mainAssemblyElement = (XmlElement)mainAssemblyElements.Item(0)!;
        string mainAssemblyDirectory = Path.GetDirectoryName(mainAssemblyElement.InnerText)!;
        string[] dllFilePaths = Directory.GetFiles(mainAssemblyDirectory).Where(f => Path.GetExtension(f) == ".dll").ToArray(); ;
        List<XmlElement> toRemoves = new List<XmlElement>();
        foreach (XmlElement child in root.ChildNodes)
        {
            if (child.Name == "Assembly")
            {
                toRemoves.Add(child);
            }
        }
        foreach (XmlElement toRemove in toRemoves)
        {
            root.RemoveChild(toRemove);
        }
        var mainAsemblyRelativeElements = root.GetElementsByTagName("Main_Assembly_EnforceRelativePath");
        if (mainAsemblyRelativeElements.Count < 1)
        {
            throw new Exception($"Failed to find element \"Main_Assembly_EnforceRelativePath\" in .nrproj file \"{nrprojPath}\")");
        }
        XmlElement elementToAppendAssembliesAfter = (XmlElement)mainAsemblyRelativeElements.Item(0)!;
        foreach (string dllFilePath in dllFilePaths)
        {
            XmlElement assemblyElement = doc.CreateElement("Assembly");
            XmlElement filenameElement = doc.CreateElement("Filename");
            filenameElement.InnerText = dllFilePath;
            assemblyElement.AppendChild(filenameElement);
            XmlElement enforceRelativePathElement = doc.CreateElement("EnforceRelativePath");
            enforceRelativePathElement.InnerText = "false";
            assemblyElement.AppendChild(enforceRelativePathElement);
            root.InsertAfter(assemblyElement, elementToAppendAssembliesAfter);
        }
        string newDocumentString = XmlHelper.ToNicelyFormattedString(doc);
        File.WriteAllText(nrprojPath, newDocumentString);
    }
    private static void BuildDotNet(string solutionPath)
    {
        StringBuilder sb = new StringBuilder();
        var processStartInfo = new ProcessStartInfo()
        {
            FileName = "dotnet.exe",
            Arguments = $"build --configuration Release {solutionPath}",
            UseShellExecute = true
        };
        Process process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new Exception("Failed to Build");
    }
    private static bool AskYesNo(string question, bool def)
    {
        Console.WriteLine($"{question} y/n (default {(def ? "y" : "n")})");
        char c = Console.ReadKey().KeyChar;
        Console.WriteLine();
        switch (c)
        {
            case 'y':
            case 'Y':
                return true;
            case 'n':
            case 'N':
                return false;
            default:
                return def;
        }
    }
    private static bool AskToType(string question, string expectedReply)
    {
        Console.WriteLine($"{question}? Type \"{expectedReply}\" and hit enter to confirm!");
        return Console.ReadLine()==expectedReply;
    }

}
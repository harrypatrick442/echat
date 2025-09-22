using Renci.SshNet;
using SSH;
using System.Diagnostics;
using System.Text;
using Core.Processes;
using RemoteAssistantCore;
using Setup.Git;
using System.Data.SqlTypes;
using System.Xml;
using Core.FileSystem;
using Core;
public class Program
{
    private const string DOTNET_REACTOR_PATH = "C:\\Program Files (x86)\\Eziriz\\.NET Reactor\\dotNET_Reactor.Console.exe";
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Which project? \"fr\" for filesrelay, \"ec\" for echat, \"rc\" for retrocause....");
            string str = Console.ReadLine();
            switch (str)
            {
                case "fr":
                    FilesRelay();
                    return;
                case "ec":
                    EChat();
                    return;
                case "rc":
                    Retrocause();
                    return;
            }
        }
    }
    private static void EChat()
    {
        bool doBackend = AskYesNo("Build and deploy backend?", true);
        bool doClient = AskYesNo("Build and deploy client?", true);
        bool doCleanupServerLogs = AskYesNo("Cleanup server logs?", true);
        bool doRetrySslCertificateIfFailed = AskYesNo("Retry SSL certificate if failed?", true);
        bool doRestartServersAfter = AskYesNo("Restart servers after?", true);
        bool doInstallFfmpeg = AskYesNo("Install ffmpeg?", false);
        if (doBackend)
        {
            Build();
            Action<string> protect = GetProtect(DOTNET_REACTOR_PATH);
            protect("C:\\repos\\snippets\\DotNetReactor\\FileServer.nrproj");
            protect("C:\\repos\\snippets\\DotNetReactor\\EChat1.nrproj");
            protect("C:\\repos\\snippets\\DotNetReactor\\MultimediaServer1.nrproj");
            protect("C:\\repos\\snippets\\DotNetReactor\\LogServer.nrproj");
            PuttyCommands.AgentCopyProjetToServer("fileserver", "C:\\repos\\snippets\\Build\\FileServer\\Release\\net7.0\\FileServer_Secure", GlobalConstants.Nodes.FILE_SERVER_1);
            PuttyCommands.AgentCopyProjetToServer("echat1", "C:\\repos\\snippets\\Build\\EChat1\\Release\\net7.0\\EChat1_Secure", GlobalConstants.Nodes.ECHAT_1);
            PuttyCommands.AgentCopyProjetToServer("multimediaserver1", "C:\\repos\\snippets\\Build\\MultimediaServer1\\Release\\net7.0\\MultimediaServer1_Secure", GlobalConstants.Nodes.MULTIMEDIA_SERVER_1);
            PuttyCommands.AgentCopyProjetToServer("logserver", "C:\\repos\\snippets\\Build\\LogServer\\Release\\net7.0\\LogServer_Secure", GlobalConstants.Nodes.LOG_SERVER_1);

        }
        if (doClient)
        {
            NPMHelper.RunScript("build_echat", "C:\\repos\\snippets\\client\\");
            PuttyCommands.AgentCopyProjetToServer("client/dev.e-chat.live", "C:\\repos\\snippets\\client\\build_echat", GlobalConstants.Nodes.FILE_SERVER_1);
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
            Console.WriteLine(RunUsingSsh(GlobalConstants.Nodes.MULTIMEDIA_SERVER_1,
                new string[] {
                        "sudo apt update"
                }
            )[0]);
            Console.WriteLine(RunUsingSsh(GlobalConstants.Nodes.MULTIMEDIA_SERVER_1,
                new string[] {
                        "sudo apt --yes --force-yes install ffmpeg"
                }
            )[0]);
            Console.WriteLine(RunUsingSsh(GlobalConstants.Nodes.MULTIMEDIA_SERVER_1,
                new string[] {
                        "ffmpeg -version"
                }
            )[0]);
        }
        if (commands.Count > 0) {
            foreach (int nodeId in new int[] {
                GlobalConstants.Nodes.FILE_SERVER_1 ,
            //      Constants.Nodes.FILES_RELAY_2 ,
                GlobalConstants.Nodes.ECHAT_1,
                GlobalConstants.Nodes.MULTIMEDIA_SERVER_1,
                GlobalConstants.Nodes.LOG_SERVER_1
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
    private static void FilesRelay() {

        Build();
        Action<string> protect = GetProtect(DOTNET_REACTOR_PATH);
        protect("C:\\repos\\snippets\\DotNetReactor\\FileServer.nrproj");
        //protect("C:\\repos\\snippets\\DotNetReactor\\FileServer2.nrproj");
        protect("C:\\repos\\snippets\\DotNetReactor\\FilesRelay.nrproj");
        //protect("C:\\repos\\snippets\\DotNetReactor\\FilesRelay2.nrproj");
        protect("C:\\repos\\snippets\\DotNetReactor\\TransferServer.nrproj");
        protect("C:\\repos\\snippets\\DotNetReactor\\TransferServer2.nrproj");
        protect("C:\\repos\\snippets\\DotNetReactor\\TransferServer3.nrproj");
        protect("C:\\repos\\snippets\\DotNetReactor\\LogServer.nrproj");
        PuttyCommands.AgentCopyProjetToServer("fileserver", "C:\\repos\\snippets\\Build\\FileServer\\Release\\net7.0\\FileServer_Secure", GlobalConstants.Nodes.FILE_SERVER_1);
        //PuttyCommands.AgentCopyProjetToServer("fileserver2", "C:\\repos\\snippets\\Build\\FileServer2\\Release\\net7.0\\FileServer2_Secure", Constants.Nodes.FILE_SERVER_2);
        PuttyCommands.AgentCopyProjetToServer("filesrelay", "C:\\repos\\snippets\\Build\\FilesRelay\\Release\\net7.0\\FilesRelay_Secure", GlobalConstants.Nodes.FILES_RELAY_1);
        //PuttyCommands.AgentCopyProjetToServer("filesrelay2", "C:\\repos\\snippets\\Build\\FilesRelay2\\Release\\net7.0\\FilesRelay2_Secure", Constants.Nodes.FILES_RELAY_2);
        PuttyCommands.AgentCopyProjetToServer("transferserver", "C:\\repos\\snippets\\Build\\TransferServer\\Release\\net7.0\\TransferServer_Secure", GlobalConstants.Nodes.TRANSFER_SERVER_1);
        PuttyCommands.AgentCopyProjetToServer("transferserver2", "C:\\repos\\snippets\\Build\\TransferServer2\\Release\\net7.0\\TransferServer2_Secure", GlobalConstants.Nodes.TRANSFER_SERVER_2);
        PuttyCommands.AgentCopyProjetToServer("transferserver3", "C:\\repos\\snippets\\Build\\TransferServer3\\Release\\net7.0\\TransferServer3_Secure", GlobalConstants.Nodes.TRANSFER_SERVER_3);
        PuttyCommands.AgentCopyProjetToServer("logserver", "C:\\repos\\snippets\\Build\\LogServer\\Release\\net7.0\\LogServer_Secure", GlobalConstants.Nodes.LOG_SERVER_1);
        string[] commands = new string[]{
                "rm -rf /var/log/syslog",
                "rm -rf /var/log/snippets.log",
                "rm -rf /db/DontTryCertifyAgain.json",
                "sudo shutdown -r"
            };
        foreach (int nodeId in new int[] {
                GlobalConstants.Nodes.FILES_RELAY_1 ,
          //      Constants.Nodes.FILES_RELAY_2 ,
                GlobalConstants.Nodes.TRANSFER_SERVER_1,
                GlobalConstants.Nodes.TRANSFER_SERVER_2,
                GlobalConstants.Nodes.TRANSFER_SERVER_3,
                GlobalConstants.Nodes.LOG_SERVER_1,
                GlobalConstants.Nodes.FILE_SERVER_1,
            //    Constants.Nodes.FILE_SERVER_2
            })
        {
            Console.WriteLine($"Now doing node {nodeId}");
            string[] results = RunUsingSsh(nodeId,
                commands
            );
            foreach (string result in results)
                Console.WriteLine(result);
        }
    }
    private static void Retrocause()
    {
        bool doBackend = AskYesNo("Build and deploy backend?", true);
        bool doCleanupServerLogs = AskYesNo("Cleanup server logs?", true);
        bool doRetrySslCertificateIfFailed = AskYesNo("Retry SSL certificate if failed?", true);
        bool doRestartServersAfter = AskYesNo("Restart servers after?", true);
        if (doBackend)
        {
            Build();
            Action<string> protect = GetProtect(DOTNET_REACTOR_PATH);
            protect("C:\\repos\\snippets\\DotNetReactor\\RetrocauseModerator.nrproj");
            protect("C:\\repos\\snippets\\DotNetReactor\\RetrocauseQuantus.nrproj");
            protect("C:\\repos\\snippets\\DotNetReactor\\LogServer.nrproj");
            PuttyCommands.AgentCopyProjetToServer("retrocause_moderator", "C:\\repos\\snippets\\Build\\FileServer\\Release\\net7.0\\RetrocauseModerator_Secure", GlobalConstants.Nodes.RETROCAUSE_MODERATOR);
            PuttyCommands.AgentCopyProjetToServer("retrocause_quantus", "C:\\repos\\snippets\\Build\\EChat1\\Release\\net7.0\\RetrocauseQuantus_Secure", GlobalConstants.Nodes.RETROCAUSE_QUANTUS);
            PuttyCommands.AgentCopyProjetToServer("multimediaserver1", "C:\\repos\\snippets\\Build\\MultimediaServer1\\Release\\net7.0\\MultimediaServer1_Secure", GlobalConstants.Nodes.MULTIMEDIA_SERVER_1);
            PuttyCommands.AgentCopyProjetToServer("logserver", "C:\\repos\\snippets\\Build\\LogServer\\Release\\net7.0\\LogServer_Secure", GlobalConstants.Nodes.LOG_SERVER_1);

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
        if (commands.Count > 0)
        {
            foreach (int nodeId in new int[] {
                GlobalConstants.Nodes.FILE_SERVER_1 ,
            //      Constants.Nodes.FILES_RELAY_2 ,
                GlobalConstants.Nodes.ECHAT_1,
                GlobalConstants.Nodes.MULTIMEDIA_SERVER_1,
                GlobalConstants.Nodes.LOG_SERVER_1
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
    private static string[] RunUsingSsh(int nodeId, params string[] commands)

    {

        var user = "root";
        var host = $"{GlobalConstants.Nodes.GetIpOrDomainForNode(nodeId)}";

        var regularKey = File.ReadAllBytes(GlobalConstants.Keys.KEY_PATH_OPENSSH);
        var pkRsa = new PrivateKeyFile(new MemoryStream(regularKey));

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

    private static Action<string> GetProtect(string dotnetReactorPath)
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
    private static void UpdateNrprojFile(string nrprojPath) {
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
        foreach (string dllFilePath in dllFilePaths) {
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
    private static void Build() {

        string solutionPath = "C:\\repos\\snippets\\API\\WebAPI.sln";
        StringBuilder sb = new StringBuilder();
        var processStartInfo = new ProcessStartInfo()
        {
            FileName = "dotnet.exe",
            Arguments = $"build --configuration Release {solutionPath}",
            UseShellExecute=true
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
        Console.WriteLine($"{question} y/n (default {(def?"y":"n")})");
        char c= Console.ReadKey().KeyChar;
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

}
using System.Text;

class Program
{
    private const bool USE_EXISTING = true;
    public static void Main(string[] args)
    {
        Configurations.Initializer.Initialize();
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string projectDirectory = Path.Combine(Directory.GetParent(Path.GetDirectoryName(exePath))
            .Parent.Parent.Parent.FullName, "Nodes");
        string generatedNodesCsPath = Path.Combine(projectDirectory, "GeneratedNodesJSON.cs");
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("namespace Nodes{");
        sb.AppendLine("public static class GeneratedNodesJSON{");
        Action<string> generate = (name) =>
        {

            string fileName = $"{name}.json";
            string networkJsonPath = Path.Combine(projectDirectory, "Setups", "Network", fileName);
            string nodesJsonPath = Path.Combine(projectDirectory, "Setups", "Nodes", fileName);
            string nodesJSON = NodesJSONGenerator.GenerateJSON(
                networkJsonPath, nodesJsonPath, USE_EXISTING);
            Console.WriteLine(nodesJSON);
            JSONProperty(sb, name, nodesJSON);
        };
        generate("Development");
        generate("Live");

        sb.AppendLine("\t}");
        sb.AppendLine("}");
        File.WriteAllText(generatedNodesCsPath, sb.ToString());
    }
    private static void JSONProperty(StringBuilder sb, string name, string nodesJSON)
    {

        sb.Append("\t\tpublic static string ");
        sb.Append(name);
        sb.AppendLine("{");
        sb.AppendLine("\t\t\tget{");
        sb.Append("\t\t\t\treturn ");
        sb.Append(Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(nodesJSON, true));
        sb.AppendLine(";");
        sb.AppendLine("\t\t\t}");
        sb.AppendLine("\t\t}");
    }
}
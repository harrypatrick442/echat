using Core.Exceptions;
using Core.Maths;
using JSON;
using Nodes;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

internal class Existing
{
    private Node[] _Nodes;
    public Existing(string nodesJSONFilePath) {
        if (File.Exists(nodesJSONFilePath)) {
            _Nodes = Json.Deserialize<Node[]>(File.ReadAllText(nodesJSONFilePath));
        }
    }
    public Existing()
    {

    }
    public InterserverConnection GetExistingInterserverConnection(int nodeId, int toNodeId) {
        return _Nodes?.Where(n => n.Id == nodeId)
            .Select(n => n.InterserverConnections
                .Where(i => i.NodeId == toNodeId)
                .FirstOrDefault()
            ).FirstOrDefault(); 
    }
}

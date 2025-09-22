// See https://aka.ms/new-console-template for more information
using Nodes;
using JSON;
using System.Reflection;
using System.Text;

public static class NodesJSONGenerator
{
    public static string GenerateJSON(string networkJsonPath, string nodesJsonPath, bool useExisting)
    {
        string networkJsonContent = File.ReadAllText(networkJsonPath);
        NetworkJSONEntry[] entries = Json.Deserialize<NetworkJSONEntry[]>(networkJsonContent);
        int[] nodesWithDuplicates = entries.GroupBy(e => e.Id).Where(g => g.Count() > 1).Select(g => g.First().Id).ToArray();
        if (nodesWithDuplicates.Length > 0)
            throw new Exception($"Duplicate entries for {string.Join(',', nodesWithDuplicates)}");
        foreach (NetworkJSONEntry entry in entries)
        {
            if (entry.To == null) throw new Exception($"{entry.Id} had no to");
            int[] duplicatedTos = entry.To.GroupBy(e => e).Where(g => g.Count() > 1).Select(g => g.First()).ToArray();
            if (duplicatedTos.Length > 0)
                throw new Exception($"{entry.Id} had duplicates of to's {string.Join(',', entry.To)}");
        }
        Dictionary<int, NetworkJSONEntry> mapNodeIdToNetworkJSONEntry = entries.ToDictionary(e => e.Id, e => e);
        Dictionary<int, Dictionary<int, NetworkPair>> mapNodeIdToMapOtherNodeIdToNetworkPair = new Dictionary<int, Dictionary<int, NetworkPair>>();
        List<NetworkPair> networkPairs = new List<NetworkPair>();
        Existing existing = useExisting!=null? new Existing(nodesJsonPath) :new Existing();
        foreach (NetworkJSONEntry entry in entries)
        {
            int nodeId = entry.Id;
            foreach (int otherNodeId in entry.To)
            {
                if (mapNodeIdToMapOtherNodeIdToNetworkPair.TryGetValue(otherNodeId, out Dictionary<int, NetworkPair> mapOtherNodeIdToNetworkPair)
                        && mapOtherNodeIdToNetworkPair.TryGetValue(nodeId, out NetworkPair networkPair))
                {
                    networkPair.BHasSeenA();
                    if (!mapNodeIdToMapOtherNodeIdToNetworkPair.TryGetValue(nodeId, out mapOtherNodeIdToNetworkPair))
                    {
                        mapOtherNodeIdToNetworkPair = new Dictionary<int, NetworkPair>();
                        mapNodeIdToMapOtherNodeIdToNetworkPair[nodeId] = mapOtherNodeIdToNetworkPair;
                    }
                    mapOtherNodeIdToNetworkPair[otherNodeId] = networkPair;
                }
                else
                {
                    if (mapNodeIdToNetworkJSONEntry.TryGetValue(otherNodeId, out NetworkJSONEntry otherNetworkJSONEntry))
                    {
                        if (!mapNodeIdToMapOtherNodeIdToNetworkPair.TryGetValue(nodeId, out mapOtherNodeIdToNetworkPair))
                        {
                            mapOtherNodeIdToNetworkPair = new Dictionary<int, NetworkPair>();
                            mapNodeIdToMapOtherNodeIdToNetworkPair[nodeId] = mapOtherNodeIdToNetworkPair;
                        }
                        networkPair = new NetworkPair(entry, otherNetworkJSONEntry, existing);
                        mapOtherNodeIdToNetworkPair[otherNodeId] = networkPair;
                        networkPairs.Add(networkPair);
                    }
                    else
                    {
                        throw new Exception($"Node {nodeId} had to {otherNodeId} which doesn't exist as an entry");
                    }
                }
            }
        }
        foreach (NetworkPair networkPair in networkPairs)
        {
            if (!networkPair.BSeenA)
                throw new Exception($"{networkPair.B.Id} has no to {networkPair.A.Id} but {networkPair.A.Id} has to {networkPair.B.Id}");
        }
        List<Node> nodes = new List<Node>();
        foreach (NetworkJSONEntry entry in entries)
        {
            List<InterserverConnection> interserverConnections = new List<InterserverConnection>();
            if (entry.To.Length > 0)
            {
                Dictionary<int, NetworkPair> mapOtherNodeIdToNetworkPair = mapNodeIdToMapOtherNodeIdToNetworkPair[entry.Id];
                foreach (var otherNodeIdNetworkPair in mapOtherNodeIdToNetworkPair)
                {
                    int otherNodeId = otherNodeIdNetworkPair.Key;
                    NetworkPair networkPair = otherNodeIdNetworkPair.Value;
                    InterserverConnection interserverConnection = new InterserverConnection(otherNodeId,
                        networkPair.GetHash(entry), networkPair.GetPassword(entry), networkPair.GetServerUrl(entry),
                        networkPair.IAmClientElseServer(entry), networkPair.GetExpectedIpAddressOfClient(entry));
                    interserverConnections.Add(interserverConnection);
                }
            }
            nodes.Add(new Node(entry.Id, interserverConnections.ToArray()));

        }
        string json = Json.Serialize(nodes.ToArray(), prettify: true);
        File.WriteAllText(nodesJsonPath, json);
        return json;
    }
}


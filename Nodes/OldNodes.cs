using SnippetsCore.Exceptions;
using SnippetsCore.Json;
using SnippetsCore.Interfaces;
using SnippetsCore;
using Logging;
using System.Diagnostics;

namespace Nodes
{
    public class Nodes:INodes
    {
        public event EventHandler<NodeEventArgs> OnNodeAdded;
        public event EventHandler<NodeEventArgs> OnNodeRemoved;
        private static Nodes _Instance;
        private Dictionary<long, Node> _MapNodeIdToNode = new Dictionary<long, Node>();
        private List<NodeIdRangePair> _OrderedNodeIdRangePairs = new List<NodeIdRangePair>();
        private readonly object _LockObjectMe = new object();
        private Node _Me;

        public static Nodes Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(typeof(Nodes));
            Logs.Default.Info("about to call nodes constructor");
            _Instance = new Nodes();
            Logs.Default.Info($"Initialized {nameof(Nodes)}");
            return _Instance;
        }
        public static Nodes Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(typeof(Nodes));
                return _Instance;
            }
        }
        public Node Me { 
            get {
                lock (_LockObjectMe)
                {
                    return _Me;
                }
            }
        }

        INode INodes.Me { get {
                lock (_LockObjectMe) {
                    return _Me;
                }
            } 
        }

        private Nodes()
        {
            Logs.Default.Info("about to called nodes constructor");
            string content =
#if DEBUG
                System.Text.Encoding.UTF8.GetString(Resource1.Nodes_Debug);
#else
                System.Text.Encoding.UTF8.GetString(Resource1.Nodes);
#endif
            if (string.IsNullOrEmpty(content)) throw new ParseException("Invalid nodes content. The string was null or empty!");
            UpdateNodesFromContent(content);
        }

        #region Methods
        #region Public
        public INode[] AsArray()
        {
            lock (_MapNodeIdToNode)
            {
                return _MapNodeIdToNode.Values.ToArray();
            }
        }
        public INode GetNodeById(long nodeId) {
            lock (_MapNodeIdToNode) {
                if (!_MapNodeIdToNode.TryGetValue(nodeId, out Node value))
                    throw new KeyNotFoundException($"nodeId {nodeId}");
                return value;
            }
        }
        public INode GetNodeForIdInRange(long snippetId) {

            lock (_OrderedNodeIdRangePairs)
            {
                for (int i = 0; i < _OrderedNodeIdRangePairs.Count; i++)
                {
                    NodeIdRangePair nodeIdRangePair = _OrderedNodeIdRangePairs[i];
                    if (snippetId < nodeIdRangePair.NodeIdRange.IdToExclusive)
                    {
                        if (snippetId < nodeIdRangePair.NodeIdRange.IdFromInclusive)
                            throw new FatalException("Something went very wrong");
                        return nodeIdRangePair.Node;
                    }
                }
                return null;
            }
        }
        public NodeAndAssociatedIds[] GetNodesForIdsInRange(long[] ids, List<long> missingIds = null)
        {
            int idsLength = ids.Length;
            if (idsLength < 1) return null;
            HashSet<NodeAndAssociatedIds> nodes = new HashSet<NodeAndAssociatedIds>();
            NodeAndAssociatedIds lastNodeAndAssociatedIds = null;
            ids = ids.OrderBy(id => id).ToArray();
            int nodeIndex = 0;
            int idIndex = 0;
            lock (_OrderedNodeIdRangePairs)
            {
                int nodesLength = _OrderedNodeIdRangePairs.Count;
                while (idIndex < idsLength)
                {
                    long id = ids[idIndex];
                    while (true)
                    {
                        if (nodeIndex >= nodesLength)
                        {
                            if (missingIds != null)
                                missingIds.Add(id);
                            break;
                        }
                        NodeIdRangePair nodeIdRangePair = _OrderedNodeIdRangePairs[nodeIndex];
                        if (id < nodeIdRangePair.NodeIdRange.IdToExclusive)
                        {
                            if (id < nodeIdRangePair.NodeIdRange.IdFromInclusive)
                            {
                                if (missingIds != null)
                                    missingIds.Add(id);
                                break;
                            }
                            if (lastNodeAndAssociatedIds != null && lastNodeAndAssociatedIds.Node.Id == nodeIdRangePair.Node.Id)
                            {
                                lastNodeAndAssociatedIds.Add(id);
                                break;
                            }
                            nodes.Add(new NodeAndAssociatedIds(nodeIdRangePair.Node, id));
                            break;
                        }
                        nodeIndex++;
                    }
                    idIndex++;
                }
            }
            return nodes.ToArray();
        }
        #endregion Public
        #region Private
        private void UpdateNodesFromContent(string content)
        {
            Node[] newNodes = NativeJsonParser.Instance.Deserialize<Node[]>(content);
            HashSet<long> unseenNodeIds = new HashSet<long>(_MapNodeIdToNode.Keys);
            lock (_MapNodeIdToNode)
            {
                lock (_OrderedNodeIdRangePairs)
                {
                    foreach (Node node in newNodes)
                    {
                        Logs.Default.Info("new node id is " + node.Id);
                        Logs.Default.Info("new node interserverconnections is null " + (node.InterserverConnections==null));
                        unseenNodeIds.Remove(node.Id);
                        if (_MapNodeIdToNode.TryGetValue(node.Id, out Node existingNode))
                        {
                            UpdateNode(existingNode, node);
                        }
                        else
                        {
                            AddNewNode(node);
                            if (node.Id != NodeConfiguration.Instance.Id) continue;
                            lock (_LockObjectMe)
                            {
                                _Me = node;
                            }
                        }
                    }
                    lock (_LockObjectMe)
                    {
                        if (_Me == null) throw new NullReferenceException($"Had no node for me with {nameof(NodeConfiguration.Instance.Id)} {NodeConfiguration.Instance.Id}");
                    }
                    foreach (long nodeIdToRemove in unseenNodeIds)
                        RemoveNode(nodeIdToRemove);
                }
            }
        }
        private void UpdateNode(Node existingNode, Node newNode) {
            existingNode.Update(newNode);
            RemoveNodeIdRanges(existingNode.Id);
            AddNodeIdRanges(newNode);
        }
        private void AddNewNode(Node node) {
            _MapNodeIdToNode[node.Id] = node;
            AddNodeIdRanges(node);
            OnNodeAdded?.Invoke(this, new NodeEventArgs(node));
        }
        private void RemoveNode(long nodeId)
        {
            Node node = _MapNodeIdToNode[nodeId];
            if (node==null) return;
            _MapNodeIdToNode.Remove(nodeId);
            RemoveNodeIdRanges(nodeId);
            OnNodeRemoved?.Invoke(this, new NodeEventArgs(node));
        }
        private void RemoveNodeIdRanges(long nodeId) {
            Iterator<List<NodeIdRangePair>, NodeIdRangePair> iterator = new Iterator<List<NodeIdRangePair>, NodeIdRangePair>(_OrderedNodeIdRangePairs);
            while (iterator.HasNext) {
                NodeIdRangePair nodeIdRangePair = iterator.Next();
                if(nodeIdRangePair.Node.Id==nodeId)
                    iterator.Remove();
            }
        }
        private void AddNodeIdRanges(Node node) {
            NodeIdRangePair[] newNodeIdRangePairsToInsert = node.IdRanges
                .Select(idRange => new NodeIdRangePair(node, idRange)).ToArray();
            _OrderedNodeIdRangePairs.AddRange(newNodeIdRangePairsToInsert);
            _OrderedNodeIdRangePairs = _OrderedNodeIdRangePairs.OrderBy(nodeIdRangePair => nodeIdRangePair.NodeIdRange.IdFromInclusive).ToList();
            NodeIdRangePair previousNodeIdRangePair = _OrderedNodeIdRangePairs.First();
            foreach (NodeIdRangePair nodeIdRangePair in _OrderedNodeIdRangePairs.Skip(1)) {
                if (nodeIdRangePair.NodeIdRange.IdFromInclusive < previousNodeIdRangePair.NodeIdRange.IdToExclusive)
                    throw new ParseException($"{nameof(NodeIdRange)} {nodeIdRangePair.NodeIdRange.IdFromInclusive} - {nodeIdRangePair.NodeIdRange.IdToExclusive} on {nameof(Node)} {node.Id} overlapped {previousNodeIdRangePair.NodeIdRange.IdFromInclusive} - {previousNodeIdRangePair.NodeIdRange.IdToExclusive} on {nameof(Node)} {previousNodeIdRangePair.Node.Id}");
            }
        }
        #endregion Private
        #endregion Methods
    }
}

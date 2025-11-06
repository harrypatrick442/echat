using Core.Exceptions;
using Initialization.Exceptions;
using JSON;

namespace Nodes
{
    public sealed class Nodes:INodes
    {
        public int MyId { get; }
        private Dictionary<int, Node> _MapNodeIdToNode;
        private Node _Me;
        private static Nodes _Instance;
        public static Nodes Initialize(int myId, string nodesJson)
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(typeof(Nodes));
            _Instance = Nodes.FromNodesJSON(myId, nodesJson);
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
        private Nodes(Node[] nodes, int myId)
        {
            _MapNodeIdToNode = nodes.ToDictionary(n => n.Id, n => n);
            MyId = myId;
            _Me = nodes.Where(n => n.Id == 
                myId).FirstOrDefault();
            if (_Me == null)
                throw new NullReferenceException($"Had no node for me with {nameof(MyId)} {myId}");
        }
        public Node Me { 
            get {
                return _Me;
            }
        }

        INode INodes.Me { get {
                return _Me;
            } 
        }

        #region Methods
        #region Public
        public INode[] AsArray()
        {
            //lock (_MapNodeIdToNode){
                return _MapNodeIdToNode.Values.ToArray();
            //}
        }
        public INode GetNodeById(int nodeId) {
            //lock (_MapNodeIdToNode) {
                if (!_MapNodeIdToNode.TryGetValue(nodeId, out Node value))
                    throw new KeyNotFoundException($"nodeId {nodeId}");
                return value;
            //}
        }
        public INode[] GetNodesConnectedToNode(int nodeId)
        {
            List<INode> otherNodes = new List<INode>();
            foreach (int otherNodeId in GetNodeById(nodeId).InterserverConnections.Select(i => i.NodeId))
            {
                otherNodes.Add(GetNodeById(otherNodeId));
            }
            return otherNodes.ToArray();
        }
        public int[] GetNodeIdsConnectedToNode(int nodeId)
        {
            return GetNodeById(nodeId).InterserverConnections.Select(i => i.NodeId).ToArray();
        }
        #endregion Public
        #region Private
        private static Nodes FromNodesJSON(int myId, string nodesJson)
        {
            if (string.IsNullOrEmpty(nodesJson)) 
                throw new ParseException("Invalid Nodes JSON content. The string was null or empty!");
            Node[] nodes = Json.Deserialize<Node[]>(nodesJson);
            return new Nodes(nodes, myId);
        }
        #endregion Private
        #endregion Methods
    }
}

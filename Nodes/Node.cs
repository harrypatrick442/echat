using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nodes
{
    [DataContract]
    public class Node : INode
    {
        //public event EventHandler<ItemEventArgs<InterserverConnection>> OnInterserverConnectionAdded;
        //public event EventHandler<ItemEventArgs<long>> OnInterserverConnectionRemoved;

        [JsonPropertyName(NodeDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = NodeDataMemberNames.Id, EmitDefaultValue =false)]
        public int Id { get; protected set; }
        [JsonIgnore]
        public bool IsMe { get { return Nodes.Instance.MyId == Id; } }
        [JsonIgnore]
        public int[] AssociatedIdTypes { get; }
        [JsonIgnore]
        private Dictionary<int, InterserverConnection> _MapNodeIdToInterserverConnection;
        [JsonPropertyName(NodeDataMemberNames.InterserverConnections)]
        [JsonInclude]
        [DataMember(Name = NodeDataMemberNames.InterserverConnections)]
        public InterserverConnection[] InterserverConnections { 
            get { 
                return _MapNodeIdToInterserverConnection?.Values.ToArray(); 
            }
            protected set {
                _MapNodeIdToInterserverConnection = value==null?new Dictionary<int, InterserverConnection>():value.ToDictionary(interserverConnection => interserverConnection.NodeId, interserverConnection => interserverConnection);
            }
        }
        public Node(int id, InterserverConnection[] interserverConnections) {
            Id = id;
            InterserverConnections = interserverConnections;
            AssociatedIdTypes = GlobalConstants.Nodes.GetAssociatedIdTypes(id);
        }
        protected Node() { }
        /*
        private IEndpoint _Endpoint;


        public IEndpoint Endpoint
        {
            get { return _Endpoint; }
            protected set
            {
                    _Endpoint = value;
            }
        }*/
        public InterserverConnection GetInterserverConnectionTo(int nodeId)
        {
            if (!_MapNodeIdToInterserverConnection.TryGetValue(nodeId, out InterserverConnection interserverConnection))
                return null;
            return interserverConnection;
        }
    }
}

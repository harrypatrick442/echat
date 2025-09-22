using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users
{
    public class NodeIdAssociatesPair
    {
        private int _NodeId;
        [JsonPropertyName(NodeIdAssociatesPairDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = NodeIdAssociatesPairDataMemberNames.NodeId)]
        public int NodeId { get { return _NodeId; } protected set { _NodeId = value; } }
        private Associate[] _Associates;
        [JsonPropertyName(NodeIdAssociatesPairDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = NodeIdAssociatesPairDataMemberNames.NodeId)]
        public Associate[] Associates { get { return _Associates; } protected set { _Associates = value; } }
        public NodeIdAssociatesPair(int nodeId, Associate[] associates)
        {
            _NodeId = nodeId;
            _Associates = associates;
        }
    }
}

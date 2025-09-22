using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRanges
{
    [DataContract]
    public class NodeIdRanges
    {
        //CHECKED
        [JsonPropertyName(NodeIdRangesDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = NodeIdRangesDataMemberNames.NodeId)]
        public int NodeId { get; protected set; }
        [JsonPropertyName(NodeIdRangesDataMemberNames.IdRanges)]
        [JsonInclude]
        [DataMember(Name = NodeIdRangesDataMemberNames.IdRanges)]
        public IdRange[] IdRanges { get; protected set; }
        public NodeIdRanges(int nodeId, IdRange[] idRanges) {
            NodeId = nodeId;
            IdRanges=idRanges;
        }
        protected NodeIdRanges() { }
    }
}

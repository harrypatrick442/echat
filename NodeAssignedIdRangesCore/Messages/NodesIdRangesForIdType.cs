using NodeAssignedIdRanges;
using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesCore.Requests
{
    [DataContract]
    public class NodesIdRangesForIdType
    {
        //CHECKED
        [JsonPropertyName(NodesIdRangesForIdTypeDataMemberNames.IdType)]
        [JsonInclude]
        [DataMember(Name = NodesIdRangesForIdTypeDataMemberNames.IdType)]
        public int IdType { get; protected set; }
        [JsonPropertyName(NodesIdRangesForIdTypeDataMemberNames.NodeIdRangess )]
        [JsonInclude]
        [DataMember(Name = NodesIdRangesForIdTypeDataMemberNames.NodeIdRangess )]
        public NodeIdRanges[] NodeIdRangess { get; protected set; }
        public NodesIdRangesForIdType(int idType, NodeIdRanges[] nodeIdRangess)
        {
            IdType = idType;
            NodeIdRangess = nodeIdRangess;
        }
        protected NodesIdRangesForIdType() { }
    }
}

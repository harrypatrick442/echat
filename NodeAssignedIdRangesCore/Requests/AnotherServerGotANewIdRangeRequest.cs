using Core.Messages.Messages;
using MessageTypes.Internal;
using NodeAssignedIdRanges;
using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Requests;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesCore.Requests
{
    [DataContract]
    public class AnotherServerGotANewIdRangeRequest:TicketedMessageBase
    {
        //CHECKED
        [JsonPropertyName(AnotherServerGotANewIdRangeRequestDataMemberNames.IdType)]
        [JsonInclude]
        [DataMember(Name = AnotherServerGotANewIdRangeRequestDataMemberNames.IdType)]
        public int IdType { get; protected set; }
        [JsonPropertyName(AnotherServerGotANewIdRangeRequestDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = AnotherServerGotANewIdRangeRequestDataMemberNames.NodeId)]
        public int NodeId { get; protected set; }
        [JsonPropertyName(AnotherServerGotANewIdRangeRequestDataMemberNames.NodeIdRange)]
        [JsonInclude]
        [DataMember(Name = AnotherServerGotANewIdRangeRequestDataMemberNames.NodeIdRange)]
        public IdRange NodeIdRange { get; protected set; }
        public AnotherServerGotANewIdRangeRequest(int idType, int nodeId, IdRange nodeIdRange)
            :base(InterserverMessageTypes.IdAnotherServerGotANewIdRange)
        {
            IdType = idType;
            NodeId = nodeId;
            NodeIdRange = nodeIdRange;
        }
        protected AnotherServerGotANewIdRangeRequest()
            : base(InterserverMessageTypes.IdAnotherServerGotANewIdRange) { }
    }
}

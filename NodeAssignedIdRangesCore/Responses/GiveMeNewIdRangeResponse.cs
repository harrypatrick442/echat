using Core.DataMemberNames;
using Core.Messages.Messages;
using NodeAssignedIdRanges;
using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Responses;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesCore.Responses
{
    [DataContract]
    public class GiveMeNewIdRangeResponse:TicketedMessageBase
    {
        //CHECKED
        [JsonPropertyName(GiveMeNewIdRangeResponseDataMemberNames.NodeIdRange)]
        [JsonInclude]
        [DataMember(Name = GiveMeNewIdRangeResponseDataMemberNames.NodeIdRange)]
        public IdRange NodeIdRange { get; protected set; }
        public GiveMeNewIdRangeResponse(IdRange nodeIdRange, long ticket)
            :base(TicketedMessageType.Ticketed)
        {
            NodeIdRange = nodeIdRange;
            Ticket = ticket;
        }
        protected GiveMeNewIdRangeResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

using Core.DataMemberNames;
using Core.Messages.Messages;
using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Responses;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesCore.Responses
{
    [DataContract]
    public class AcknowledgeResponse:TicketedMessageBase
    {
        [JsonPropertyName(AcknowledgeResponseDataMemberNames.Understood)]
        [JsonInclude]
        [DataMember(Name = AcknowledgeResponseDataMemberNames.Understood)]
        public bool Understood { get; protected set; }
        public AcknowledgeResponse(bool understood, long ticket)
            :base(TicketedMessageType.Ticketed)
        {
            Understood = understood;
            Ticket = ticket;
        }
        protected AcknowledgeResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

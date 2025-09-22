using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class UnbanUserResponse : TicketedMessageBase
    {
        [JsonPropertyName(UnbanUserResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = UnbanUserResponseDataMemberNames.FailedReason)]
        public AdministratorsFailedReason? FailedReason { get; protected set; }
        public UnbanUserResponse(AdministratorsFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected UnbanUserResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

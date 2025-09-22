using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class BanUserResponse : TicketedMessageBase
    {
        [JsonPropertyName(BanUserResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = BanUserResponseDataMemberNames.FailedReason)]
        public AdministratorsFailedReason? FailedReason { get; protected set; }
        public BanUserResponse(AdministratorsFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected BanUserResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

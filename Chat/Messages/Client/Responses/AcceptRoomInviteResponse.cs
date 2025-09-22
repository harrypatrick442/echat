using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class AcceptRoomInviteResponse : TicketedMessageBase
    {
        [JsonPropertyName(AcceptRoomInviteResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = AcceptRoomInviteResponseDataMemberNames.FailedReason)]
        public JoinFailedReason? FailedReason{ get; protected set; }
        public AcceptRoomInviteResponse(JoinFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected AcceptRoomInviteResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

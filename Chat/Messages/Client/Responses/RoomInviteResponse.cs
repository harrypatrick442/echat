using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class RoomInviteResponse : TicketedMessageBase
    {
        [JsonPropertyName(RoomInviteResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = RoomInviteResponseDataMemberNames.FailedReason)]
        public InviteFailedReason? FailedReason
        {
            get;
            protected set;
        }
        public RoomInviteResponse(
            InviteFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            _Ticket = ticket;
        }
        protected RoomInviteResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

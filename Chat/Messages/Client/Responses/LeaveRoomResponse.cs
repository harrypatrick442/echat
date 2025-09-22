using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class LeaveRoomResponse : TicketedMessageBase
    {
        [JsonPropertyName(LeaveRoomResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = LeaveRoomResponseDataMemberNames.FailedReason)]
        public RemoveRoomUserFailedReason? FailedReason{ get; protected set; }
        public LeaveRoomResponse(RemoveRoomUserFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected LeaveRoomResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

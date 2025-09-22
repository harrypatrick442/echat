using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class RemoveUserFromRoomResponse : TicketedMessageBase
    {
        [JsonPropertyName(RemoveUserFromRoomResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromRoomResponseDataMemberNames.FailedReason)]
        public RemoveRoomUserFailedReason? FailedReason{ get; protected set; }
        public RemoveUserFromRoomResponse(RemoveRoomUserFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected RemoveUserFromRoomResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

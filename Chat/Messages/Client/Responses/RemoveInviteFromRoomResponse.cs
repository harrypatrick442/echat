using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class RemoveInviteFromRoomResponse : TicketedMessageBase
    {
        [JsonPropertyName(RemoveInviteFromRoomResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = RemoveInviteFromRoomResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public RemoveInviteFromRoomResponse(bool success, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
        protected RemoveInviteFromRoomResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

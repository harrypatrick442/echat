using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class LeaveRoomRequest : TicketedMessageBase
    {
        [JsonPropertyName(LeaveRoomRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = LeaveRoomRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(LeaveRoomRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = LeaveRoomRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(LeaveRoomRequestDataMemberNames.AllowRemoveOnlyFullAdmin)]
        [JsonInclude]
        [DataMember(Name = LeaveRoomRequestDataMemberNames.AllowRemoveOnlyFullAdmin)]
        public bool AllowRemoveOnlyFullAdmin { get; protected set; }
        public LeaveRoomRequest(long conversationId, long userId, bool allowRemoveOnlyFullAdmin)
            : base(MessageTypes.ChatLeaveRoom)
        {
            ConversationId = conversationId;
            UserId = userId;
            AllowRemoveOnlyFullAdmin = allowRemoveOnlyFullAdmin;
        }
        protected LeaveRoomRequest()
            : base(MessageTypes.ChatLeaveRoom) { }
    }
}

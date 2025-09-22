using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RemoveUserFromRoomRequest : TicketedMessageBase
    {
        [JsonPropertyName(RemoveUserFromRoomRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromRoomRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(RemoveUserFromRoomRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromRoomRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(RemoveUserFromRoomRequestDataMemberNames.AllowRemoveOnlyFullAdmin)]
        [JsonInclude]
        [DataMember(Name = RemoveUserFromRoomRequestDataMemberNames.AllowRemoveOnlyFullAdmin)]
        public bool AllowRemoveOnlyFullAdmin { get; protected set; }
        public RemoveUserFromRoomRequest(long conversationId, long userId)
            : base(InterserverMessageTypes.ChatRemoveUserFromRoom)
        {
            ConversationId = conversationId;
            UserId = userId;
        }
        protected RemoveUserFromRoomRequest()
            : base(InterserverMessageTypes.ChatRemoveUserFromRoom) { }
    }
}

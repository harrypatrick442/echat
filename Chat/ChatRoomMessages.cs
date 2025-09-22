using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using MultimediaCore;
using Chat.DataMemberNames.Messages;

namespace Chat
{
    [DataContract]
    public class ChatRoomMessages
    {
        [JsonPropertyName(ChatRoomMessagesDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ChatRoomMessagesDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(ChatRoomMessagesDataMemberNames.Messages)]
        [JsonInclude]
        [DataMember(Name = ChatRoomMessagesDataMemberNames.Messages)]
        public ClientMessage[] Messages { get; protected set; }
        [JsonPropertyName(ChatRoomMessagesDataMemberNames.Reactions)]
        [JsonInclude]
        [DataMember(Name = ChatRoomMessagesDataMemberNames.Reactions)]
        public MessageReaction[] Reactions { get; protected set; }
        [JsonPropertyName(ChatRoomMessagesDataMemberNames.UserMultimediaItems)]
        [JsonInclude]
        [DataMember(Name = ChatRoomMessagesDataMemberNames.UserMultimediaItems)]
        public MessageUserMultimediaItem[] UserMultimediaItems { get; protected set; }
        public ChatRoomMessages(long conversationId, ClientMessage[] messages, MessageReaction[] reactions,
            MessageUserMultimediaItem[] userMultimediaItems)
        {
            ConversationId = conversationId;
            Messages = messages;
            Reactions = reactions;
            UserMultimediaItems = userMultimediaItems;
        }
        protected ChatRoomMessages() { }
    }
}

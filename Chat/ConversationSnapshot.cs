using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text.Json.Serialization;
using System.Web.Services.Description;
using Chat.DataMemberNames.Messages;
using Chat.Messages.Client.Messages;
using Core.Timing;

namespace Chat
{
    [DataContract]
    public class ConversationSnapshot: ClientMessage
    {
        [JsonPropertyName(ConversationSnapshotDataMemberNames.UserIdsInConversation)]
        [JsonInclude]
        [DataMember(Name = ConversationSnapshotDataMemberNames.UserIdsInConversation)]//otherUserId
        public long[] UserIdsInConversation { get; protected set; }
        [JsonPropertyName(ConversationSnapshotDataMemberNames.Seen)]
        [JsonInclude]
        [DataMember(Name = ConversationSnapshotDataMemberNames.Seen)]//otherUserId
        public bool Seen { get; set; }
        public ConversationSnapshot(long id, int version, long sentAt, long userId, string content,
            long conversationId, ConversationType conversationType, long[] userIdsInConversation, bool seen) :
            base(id, version, sentAt, userId, content)
        {
            ConversationId = conversationId;
            ConversationType = conversationType;
            UserIdsInConversation = userIdsInConversation;
            Seen = seen;
        }
        public ConversationSnapshot(ClientMessage message, long[] userIdsInConversation, bool seen) :
            base(message.Id, message.Version, message.SentAt, message.UserId, message.Content)
        {
            ConversationId = message.ConversationId;
            ConversationType = message.ConversationType;
            UserIdsInConversation = userIdsInConversation;
            Seen = seen;
        }
        protected ConversationSnapshot() { }
        public void UpdateWithLatestMessage(
            ClientMessage message,
            long[] userIdsInConversation, bool seen)
        {
            ConversationType = message.ConversationType;
            ConversationId = message.ConversationId;
            Content = message.Content;
            SentAt = message.SentAt;
            UserId = message.UserId;
            Version = message.Version;
            UserIdsInConversation = userIdsInConversation;
            Seen = seen;
            Id = message.Id;
        }
    }
}

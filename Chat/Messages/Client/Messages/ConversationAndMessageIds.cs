using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class ConversationAndMessageIds
    {
        [JsonPropertyName(ConversationAndMessageIdsDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ConversationAndMessageIdsDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(ConversationAndMessageIdsDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = ConversationAndMessageIdsDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        [JsonPropertyName(ConversationAndMessageIdsDataMemberNames.MessageIds)]
        [JsonInclude]
        [DataMember(Name = ConversationAndMessageIdsDataMemberNames.MessageIds)]
        public long[] MessageIds{ get; protected set; }
        public ConversationAndMessageIds(long conversationId,
            ConversationType conversationType, long[] messageIds)
        {
            ConversationId = conversationId;
            ConversationType = conversationType;
            MessageIds = messageIds;
        }
        protected ConversationAndMessageIds()
        {

        }
    }
}

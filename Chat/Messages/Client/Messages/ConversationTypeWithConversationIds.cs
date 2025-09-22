using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class ConversationTypeWithConversationIds
    {
        [JsonPropertyName(ConversationTypeWithConversationIdsDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = ConversationTypeWithConversationIdsDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        [JsonPropertyName(ConversationTypeWithConversationIdsDataMemberNames.ConversationIds)]
        [JsonInclude]
        [DataMember(Name = ConversationTypeWithConversationIdsDataMemberNames.ConversationIds)]
        public long[] ConversationIds{ get; protected set; }
        public ConversationTypeWithConversationIds(ConversationType conversationType, long[] conversationIds)
        {
            ConversationType = conversationType;
            ConversationIds = conversationIds;
        }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class FetchConversationIndividualMessagesResult : TypedMessageBase
    {
        [JsonPropertyName(FetchConversationIndividualMessagesResultDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = FetchConversationIndividualMessagesResultDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(FetchConversationIndividualMessagesResultDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = FetchConversationIndividualMessagesResultDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        [JsonPropertyName(FetchConversationIndividualMessagesResultDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = FetchConversationIndividualMessagesResultDataMemberNames.FailedReason)]
        public ChatFailedReason FailedReason { get; protected set; } = ChatFailedReason.None;
        [JsonPropertyName(FetchConversationIndividualMessagesResultDataMemberNames.Messages)]
        [JsonInclude]
        [DataMember(Name = FetchConversationIndividualMessagesResultDataMemberNames.Messages)]
        public ClientMessage[] Messages { get; protected set; }
        public FetchConversationIndividualMessagesResult(long conversationId, ChatFailedReason failedReason)
        {
            ConversationId = conversationId;
            FailedReason = failedReason;
        }
        public FetchConversationIndividualMessagesResult(long conversationId, ConversationType conversationType, ClientMessage[] messages)
        {
            ConversationId = conversationId;
            ConversationType = conversationType;
            Messages = messages;
        }
    }
}

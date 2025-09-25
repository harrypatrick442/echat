using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.DTOs;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class UnreactToMessage : TypedMessageBase
    {
        [JsonPropertyName(UnreactToMessageDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = UnreactToMessageDataMemberNames.ConversationId, EmitDefaultValue =false)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(UnreactToMessageDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = UnreactToMessageDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        [JsonPropertyName(UnreactToMessageDataMemberNames.MessageReaction)]
        [JsonInclude]
        [DataMember(Name = UnreactToMessageDataMemberNames.MessageReaction)]
        public MessageReaction MessageReaction
        {
            get;
            protected set;
        }
        public UnreactToMessage(long conversationId,
            MessageReaction messageResponse)
        {
            Type = MessageTypes.ChatUnreactToMessage;
            ConversationId = conversationId;
            MessageReaction = messageResponse;
        }
        protected UnreactToMessage() { }
    }
}

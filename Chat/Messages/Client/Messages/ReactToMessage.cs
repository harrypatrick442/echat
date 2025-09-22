using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class ReactToMessage : TypedMessageBase
    {
        [JsonPropertyName(ReactToMessageDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ReactToMessageDataMemberNames.ConversationId, EmitDefaultValue =false)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(ReactToMessageDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = ReactToMessageDataMemberNames.ConversationType, EmitDefaultValue =false)]
        public ConversationType ConversationType
        {
            get;
            protected set;
        }
        [JsonPropertyName(ReactToMessageDataMemberNames.MessageReaction)]
        [JsonInclude]
        [DataMember(Name = ReactToMessageDataMemberNames.MessageReaction)]
        public MessageReaction MessageReaction
        {
            get;
            protected set;
        }
        public ReactToMessage(long conversationId, 
            ConversationType conversationType, MessageReaction messageResponse)
        {
            Type = global::MessageTypes.MessageTypes.ChatReactToMessage;
            ConversationId = conversationId;
            ConversationType = conversationType;
            MessageReaction = messageResponse;
        }
        protected ReactToMessage() { }
    }
}

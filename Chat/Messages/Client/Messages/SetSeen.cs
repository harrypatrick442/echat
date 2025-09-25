using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class SetSeenMessage : TypedMessageBase
    {
        [JsonPropertyName(SetSeenMessageDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = SetSeenMessageDataMemberNames.MyUserId)]
        public long MyUserId { get; protected set; }
        [JsonPropertyName(SetSeenMessageDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = SetSeenMessageDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(SetSeenMessageDataMemberNames.MessageId)]
        [JsonInclude]
        [DataMember(Name = SetSeenMessageDataMemberNames.MessageId)]
        public long MessageId { get; protected set; }
        public SetSeenMessage(long myUserId, long conversationId, long messageId)
        {
            MyUserId = myUserId;
            ConversationId = conversationId;
            MessageId = messageId;
            _Type = MessageTypes.ChatSetSeenMessage;
        }
        protected SetSeenMessage()
        {
            _Type = MessageTypes.ChatSetSeenMessage;
        }
    }
}

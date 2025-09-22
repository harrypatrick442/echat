using Chat.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat.Messages.Client
{
    [DataContract]
    public class MessageReaction
    {
        [JsonPropertyName(MessageReactionDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = MessageReactionDataMemberNames.UserId)]
        public long UserId
        {
            get;
            set;
        }
        [JsonPropertyName(MessageReactionDataMemberNames.MessageId)]
        [JsonInclude]
        [DataMember(Name = MessageReactionDataMemberNames.MessageId)]
        public long MessageId
        {
            get;
            protected set;
        }
        [JsonPropertyName(MessageReactionDataMemberNames.Code)]
        [JsonInclude]
        [DataMember(Name = MessageReactionDataMemberNames.Code)]
        public long? Code
        {
            get;
            protected set;
        }
        [JsonPropertyName(MessageReactionDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = MessageReactionDataMemberNames.MultimediaToken)]
        public string MultimediaToken
        {
            get;
            protected set;
        }
        public MessageReaction(long messageId, long userId, long? code, string multimediaToken)
        {
            MessageId = messageId;
            UserId = userId;
            Code = code;
            MultimediaToken = multimediaToken;
        }
        protected MessageReaction() { }
    }
}

using Core.Messages.Messages;
using MentionsCore.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MentionsCore.Messages
{
    [DataContract]
    public class Mention:TypedMessageBase
    {
        [JsonPropertyName(MentionDataMemberNames.UserIdMentioning)]
        [JsonInclude]
        [DataMember(Name = MentionDataMemberNames.UserIdMentioning)]
        public long UserIdMentioning { get; protected set; }
        [JsonPropertyName(MentionDataMemberNames.AtTime)]
        [JsonInclude]
        [DataMember(Name = MentionDataMemberNames.AtTime)]
        public long AtTime { get; protected set; }
        [JsonPropertyName(MentionDataMemberNames.MessageId)]
        [JsonInclude]
        [DataMember(Name = MentionDataMemberNames.MessageId)]
        public long MessageId { get; protected set; }
        [JsonPropertyName(MentionDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = MentionDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(MentionDataMemberNames.ContentSnapshot)]
        [JsonInclude]
        [DataMember(Name = MentionDataMemberNames.ContentSnapshot)]
        public string ContentSnapshot { get; protected set; }
        [JsonPropertyName(MentionDataMemberNames.Seen)]
        [JsonInclude]
        [DataMember(Name = MentionDataMemberNames.Seen)]
        public bool Seen { get; protected set; }
        public Mention(long userIdMentioning, long atTime, long messageId,
            long conversationId,
            string contentSnapshot, bool seen)
        {
            Type = global::MessageTypes.MessageTypes.MentionsMention;
            UserIdMentioning = userIdMentioning;
            AtTime = atTime;
            MessageId = messageId;
            ConversationId = conversationId;
            ContentSnapshot = contentSnapshot;
            Seen = seen;
        }
        protected Mention() { }
    }
}

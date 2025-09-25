using Core.Messages.Messages;
using MentionsCore.DataMemberNames.Requests;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MentionsCore.Requests
{
    [DataContract]
    public class SetSeenMention : TypedMessageBase
    {
        [JsonPropertyName(SetSeenMentionDataMemberNames.UserIdBeingMentioned)]
        [JsonInclude]
        [DataMember(Name = SetSeenMentionDataMemberNames.UserIdBeingMentioned)]
        public long UserIdBeingMentioned
        {
            get;
            protected set;
        }
        [JsonPropertyName(SetSeenMentionDataMemberNames.MessageId)]
        [JsonInclude]
        [DataMember(Name = SetSeenMentionDataMemberNames.MessageId)]
        public long MessageId
        {
            get;
            protected set;
        }
        public SetSeenMention(long userIdBeingMentioned, long messageId)
            :base()
        {
            _Type = MessageTypes.MentionsSetSeen;
            UserIdBeingMentioned = userIdBeingMentioned;
            MessageId = messageId;
        }
        protected SetSeenMention()
            : base() {
            _Type = MessageTypes.MentionsSetSeen;
        }
    }
}

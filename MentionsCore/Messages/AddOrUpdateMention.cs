using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MentionsCore.DataMemberNames.Requests;

namespace MentionsCore.Messages
{
    [DataContract]
    public class AddOrUpdateMention : TypedMessageBase
    {
        [JsonPropertyName(AddOrUpdateMentionDataMemberNames.UserIdsBeingMentioned)]
        [JsonInclude]
        [DataMember(Name = AddOrUpdateMentionDataMemberNames.UserIdsBeingMentioned)]
        public long[] UserIdsBeingMentioned
        {
            get;
            protected set;
        }
        [JsonPropertyName(AddOrUpdateMentionDataMemberNames.Mention)]
        [JsonInclude]
        [DataMember(Name = AddOrUpdateMentionDataMemberNames.Mention)]
        public Mention Mention
        {
            get;
            protected set;
        }
        [JsonPropertyName(AddOrUpdateMentionDataMemberNames.IsUpdate)]
        [JsonInclude]
        [DataMember(Name = AddOrUpdateMentionDataMemberNames.IsUpdate)]
        public bool IsUpdate
        {
            get;
            protected set;
        }
        public AddOrUpdateMention(long[] userIdsBeingMentioned, Mention mention, bool isUpdate)
            : base()
        {
            Type = global::MessageTypes.MessageTypes.MentionsAddOrUpdate;
            UserIdsBeingMentioned = userIdsBeingMentioned;
            IsUpdate = isUpdate;
            Mention = mention;
        }
        protected AddOrUpdateMention()
            : base() { }
    }
}

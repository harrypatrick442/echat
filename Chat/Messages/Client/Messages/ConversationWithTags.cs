using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class ConversationWithTags
    {
        [JsonPropertyName(ConversationWithTagsDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ConversationWithTagsDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get;
            set;
        }
        [JsonPropertyName(ConversationWithTagsDataMemberNames.ExactMatchTags)]
        [JsonInclude]
        [DataMember(Name = ConversationWithTagsDataMemberNames.ExactMatchTags)]
        public string[] ExactMatchTags
        {
            get;
            set;
        }
        [JsonPropertyName(ConversationWithTagsDataMemberNames.PartialMatchTags)]
        [JsonInclude]
        [DataMember(Name = ConversationWithTagsDataMemberNames.PartialMatchTags)]
        public string[] PartialMatchTags
        {
            get;
            set;
        }
        public ConversationWithTags(long conversationId, string[] exactMatchTags, string[] partialMatchTags) : base()
        {
            ConversationId = conversationId;
            ExactMatchTags = exactMatchTags;
            PartialMatchTags = partialMatchTags;
        }
        protected ConversationWithTags() { }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Requests;
using HashTags.Enums;

namespace HashTags.Messages
{
    [DataContract]
    public class SearchTagsRequest : TicketedMessageBase
    {
        [JsonPropertyName(SearchTagsRequestDataMemberNames.Tag)]
        [JsonInclude]
        [DataMember(Name = SearchTagsRequestDataMemberNames.Tag)]
        public string Tag { get; protected set; }
        [JsonPropertyName(SearchTagsRequestDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = SearchTagsRequestDataMemberNames.ScopeType)]
        public HashTagScopeTypes ScopeType { get; protected set; }
        [JsonPropertyName(SearchTagsRequestDataMemberNames.AllowPartialMatches)]
        [JsonInclude]
        [DataMember(Name = SearchTagsRequestDataMemberNames.AllowPartialMatches)]
        public bool AllowPartialMatches { get; protected set; }
        [JsonPropertyName(SearchTagsRequestDataMemberNames.MaxNEntries)]
        [JsonInclude]
        [DataMember(Name = SearchTagsRequestDataMemberNames.MaxNEntries)]
        public int MaxNEntries { get; protected set; }
        public SearchTagsRequest(string tag, HashTagScopeTypes scopeType, bool allowPartialMatches, int maxNEntries)
            :base(MessageTypes.SearchTags)
        {
            Tag = tag;
            ScopeType = scopeType;
            AllowPartialMatches = allowPartialMatches;
            MaxNEntries = maxNEntries;
        }
        protected SearchTagsRequest()
            : base(MessageTypes.SearchTags) { }
    }
}

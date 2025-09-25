using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Messages;
using HashTags.Enums;

namespace HashTags.Messages
{
    [DataContract]
    public class SearchTagsMultipleScopeTypesRequest : TicketedMessageBase
    {
        [JsonPropertyName(SearchTagsMultipleScopeTypesRequestDataMemberNames.Tag)]
        [JsonInclude]
        [DataMember(Name = SearchTagsMultipleScopeTypesRequestDataMemberNames.Tag)]
        public string Tag { get; protected set; }
        [JsonPropertyName(SearchTagsMultipleScopeTypesRequestDataMemberNames.ScopeTypes)]
        [JsonInclude]
        [DataMember(Name = SearchTagsMultipleScopeTypesRequestDataMemberNames.ScopeTypes)]
        public HashTagScopeTypes[] ScopeTypes{ get; protected set; }
        [JsonPropertyName(SearchTagsMultipleScopeTypesRequestDataMemberNames.AllowPartialMatches)]
        [JsonInclude]
        [DataMember(Name = SearchTagsMultipleScopeTypesRequestDataMemberNames.AllowPartialMatches)]
        public bool AllowPartialMatches { get; protected set; }
        [JsonPropertyName(SearchTagsMultipleScopeTypesRequestDataMemberNames.MaxNEntriesPerScopeType)]
        [JsonInclude]
        [DataMember(Name = SearchTagsMultipleScopeTypesRequestDataMemberNames.MaxNEntriesPerScopeType)]
        public int MaxNEntriesPerScopeType { get; protected set; }
        public SearchTagsMultipleScopeTypesRequest(string tag, HashTagScopeTypes[] scopeTypes, 
            bool allowPartialMatches, int maxNEntriesPerScopeType)
            :base(MessageTypes.SearchTagsMultipleScopeTypes)
        {
            Tag = tag;
            ScopeTypes = scopeTypes;
            AllowPartialMatches = allowPartialMatches;
            MaxNEntriesPerScopeType = maxNEntriesPerScopeType;
        }
        protected SearchTagsMultipleScopeTypesRequest()
            : base(MessageTypes.SearchTagsMultipleScopeTypes) { }
    }
}

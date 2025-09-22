using HashTags.DataMemberNames.Messages;
using HashTags.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HashTags.Messages
{
    [DataContract]
    public class SearchTagsResultForScopeType
    {
        [JsonPropertyName(SearchTagsResultForScopeTypeDataMemberNames.ExactMatches)]
        [JsonInclude]
        [DataMember(Name = SearchTagsResultForScopeTypeDataMemberNames.ExactMatches)]
        public ScopeIds[]? ExactMatches { get; protected set; }
        [JsonPropertyName(SearchTagsResultForScopeTypeDataMemberNames.PartialMatches)]
        [JsonInclude]
        [DataMember(Name = SearchTagsResultForScopeTypeDataMemberNames.PartialMatches)]
        public TagWithScopeIds[]? PartialMatches { get; protected set; }
        [JsonPropertyName(SearchTagsResultForScopeTypeDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = SearchTagsResultForScopeTypeDataMemberNames.ScopeType)]
        public HashTagScopeTypes ScopeType { get; protected set; }
        [JsonPropertyName(SearchTagsResultForScopeTypeDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = SearchTagsResultForScopeTypeDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public SearchTagsResultForScopeType(bool success, HashTagScopeTypes scopeType, ScopeIds[]? exactMatches, TagWithScopeIds[]? partialMatches)
        {
            Success = success;
            ScopeType = scopeType;
            ExactMatches = exactMatches;
            PartialMatches = partialMatches;
        }
        protected SearchTagsResultForScopeType() { }
    }
}

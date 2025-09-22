using HashTags.Messages;
using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Messages
{
    public static class SearchTagsResultForScopeTypeDataMemberNames
    {
        public const string
            Success = "a",
            ScopeType = "b";
        [DataMemberNamesClass(typeof(ScopeIdsDataMemberNames), true)]
        public const string
            ExactMatches = "c";
        [DataMemberNamesClass(typeof(TagWithScopeIdsDataMemberNames), true)]
        public const string
            PartialMatches = "d";
    }
}
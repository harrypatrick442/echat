using HashTags.DataMemberNames.Messages;
using HashTags.Messages;
using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Responses
{
    public static class SearchTagsMultipleScopeTypesResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(SearchTagsResultForScopeTypeDataMemberNames), isArray:true)]
        public const string Results = "r";
    }
}
using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Requests
{
    [MessageType(MessageTypes.SearchTags)]
    public static class SearchTagsRequestDataMemberNames
    {
        public const string
            Tag = "t",
            ScopeType = "s",
            AllowPartialMatches = "a",
            MaxNEntries = "m";
    }
}
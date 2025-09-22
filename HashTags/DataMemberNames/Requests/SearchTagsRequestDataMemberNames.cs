using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.SearchTags)]
    public static class SearchTagsRequestDataMemberNames
    {
        public const string
            Tag = "t",
            ScopeType = "s",
            AllowPartialMatches = "a",
            MaxNEntries = "m";
    }
}
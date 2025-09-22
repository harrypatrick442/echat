using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Messages
{
    [MessageType(MessageTypes.MessageTypes.SearchTagsMultipleScopeTypes)]
    public static class SearchTagsMultipleScopeTypesRequestDataMemberNames
    {
        public const string
            Tag = "t",
            ScopeTypes = "s",
            AllowPartialMatches = "a",
            MaxNEntriesPerScopeType = "m";
    }
}
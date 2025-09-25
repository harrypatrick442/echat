using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Requests
{
    [MessageType(MessageTypes.SearchToPredictTag)]
    public static class SearchToPredictTagRequestDataMemberNames
    {
        public const string
            Str = "t",
            ScopeType = "s",
            MaxNEntries = "m";
    }
}
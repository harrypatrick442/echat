using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.AddTags)]
    public static class AddTagsRequestDataMemberNames
    {
        public const string
            ScopeType = "s",
            ScopeId = "a",
            ScopeId2 = "b",
            Tags = "t";
    }
}
using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.DeleteTags)]
    public static class DeleteTagsRequestDataMemberNames
    {
        public const string
            Tags = "t",
            ScopeType = "s",
            ScopeId = "a",
            ScopeId2 = "b";
    }
}
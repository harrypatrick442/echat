using MessageTypes.Attributes;

namespace HashTags.DataMemberNames.Requests
{
    [MessageType(MessageTypes.DeleteTags)]
    public static class DeleteTagsRequestDataMemberNames
    {
        public const string
            Tags = "t",
            ScopeType = "s",
            ScopeId = "a",
            ScopeId2 = "b";
    }
}
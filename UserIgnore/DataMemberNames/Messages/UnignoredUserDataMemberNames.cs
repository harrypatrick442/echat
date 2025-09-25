using MessageTypes.Attributes;

namespace UserIgnore.DataMemberNames.Messages
{
    [MessageType(MessageTypes.UserIgnoreUnignored)]
    public static class UnignoredUserDataMemberNames
    {
        public const string UserId = "i";
    }
}
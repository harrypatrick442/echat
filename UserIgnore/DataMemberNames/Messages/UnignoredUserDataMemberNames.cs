using MessageTypes.Attributes;

namespace UserIgnore.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.UserIgnoreUnignored)]
    public static class UnignoredUserDataMemberNames
    {
        public const string UserId = "i";
    }
}
using MessageTypes.Attributes;

namespace UserIgnore.DataMemberNames.Messages
{
    [MessageType(MessageTypes.UserIgnoreIgnored)]
    public static class IgnoredUserDataMemberNames
    {
        public const string UserId = "i";
    }
}
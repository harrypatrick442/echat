using MessageTypes.Attributes;

namespace UserIgnore.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UserIgnoreUnignore)]
    public static class UnignoreUserRequestDataMemberNames
    {
        public const string UserId = "i";
    }
}
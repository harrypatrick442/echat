using MessageTypes.Attributes;

namespace UserIgnore.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UserIgnoreIgnore)]
    public static class IgnoreUserRequestDataMemberNames
    {
        public const string UserId = "i";
    }
}
using MessageTypes.Attributes;

namespace UserIgnore.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UserIgnoreIgnore)]
    public static class IgnoreUserRequestDataMemberNames
    {
        public const string UserId = "i";
    }
}
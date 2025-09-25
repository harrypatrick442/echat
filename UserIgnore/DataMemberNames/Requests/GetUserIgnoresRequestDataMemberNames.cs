using MessageTypes.Attributes;

namespace UserIgnore.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UserIgnoreGet)]
    public static class GetUserIgnoresRequestDataMemberNames
    {
        public const string UserId = "u";
    }
}
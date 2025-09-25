using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatUnbanUser)]
    public static class UnbanUserRequestDataMemberNames
    {
        public const string
            UserId = "u";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatBanUser)]
    public static class BanUserRequestDataMemberNames
    {
        public const string
            UserId = "u";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatRoomUserLeft)]
    public static class UserLeftMessageDataMemberNames
    {
        public const string
            UserId = "u";
    }
}
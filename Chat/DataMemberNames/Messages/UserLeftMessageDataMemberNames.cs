using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatRoomUserLeft)]
    public static class UserLeftMessageDataMemberNames
    {
        public const string
            UserId = "u";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatRoomUserCameOnline)]
    public static class UserCameOnlineMessageDataMemberNames
    {
        public const string
            UserId = "u",
            Timestamp = "t";
    }
}
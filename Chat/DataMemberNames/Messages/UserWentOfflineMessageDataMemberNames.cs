using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatRoomUserWentOffline)]
    public static class UserWentOfflineMessageDataMemberNames
    {
        public const string
            UserId = "u",
            Timestamp = "t";
    }
}
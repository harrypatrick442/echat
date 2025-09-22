using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatRoomUserWentOffline)]
    public static class UserWentOfflineMessageDataMemberNames
    {
        public const string
            UserId = "u",
            Timestamp = "t";
    }
}
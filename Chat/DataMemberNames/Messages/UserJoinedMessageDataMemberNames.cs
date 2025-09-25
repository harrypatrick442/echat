using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatRoomUserJoined)]
    public static class UserJoinedMessageDataMemberNames
    {
        public const string
            UserId = "u",
            Timestamp = "t";
    }
}
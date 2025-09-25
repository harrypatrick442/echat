using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatAttemptEnterRoom)]
    public static class AttemptEnterRoomMessageDataMemberNames
    {
        public const string
            Password = "p";
    }
}
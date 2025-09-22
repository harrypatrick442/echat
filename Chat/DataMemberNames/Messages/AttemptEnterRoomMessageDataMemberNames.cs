using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatAttemptEnterRoom)]
    public static class AttemptEnterRoomMessageDataMemberNames
    {
        public const string
            Password = "p";
    }
}
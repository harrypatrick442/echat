using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatFailedEnterRoom)]
    public static class FailedEnterRoomMessageDataMemberNames
    {
        public const string
            FailedReason = "r",
            JoinFailedReason = "j",
            Visibility = "v";
    }
}
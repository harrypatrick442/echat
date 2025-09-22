using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatLeaveRoom)]
    public static class LeaveRoomRequestDataMemberNames
    {
        public const string
            ConversationId = "c";
        public const string
            AllowRemoveOnlyFullAdmin = "r";
        [DataMemberNamesIgnore]
        public const string
            UserId = "u";
    }
}
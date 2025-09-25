using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatRoomInvite)]
    public static class RoomInviteRequestDataMemberNames
    {
        public const string
            RemovedElseSent = "r",
            ConversationId = "c",
            OtherUserId = "t";
        [DataMemberNamesIgnore(toJSON: true)]
        public const string MyUserId = "m";
    }
}
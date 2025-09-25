using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatCancelRoomInvite)]
    public static class CancelRoomInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserId = "u";
    }
}
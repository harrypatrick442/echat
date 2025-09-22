using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatCancelRoomInvite)]
    public static class CancelRoomInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserId = "u";
    }
}
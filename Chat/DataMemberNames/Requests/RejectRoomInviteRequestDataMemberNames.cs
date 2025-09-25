using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatRejectRoomInvite)]
    public static class RejectRoomInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c";
    }
}
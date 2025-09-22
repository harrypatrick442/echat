using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatRejectRoomInvite)]
    public static class RejectRoomInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c";
    }
}
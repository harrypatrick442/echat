using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatAcceptRoomInvite)]
    public static class AcceptRoomInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserId = "u";
    }
}
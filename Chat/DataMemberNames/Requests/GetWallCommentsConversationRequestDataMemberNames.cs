using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatGetWallCommentsConversation)]
    public static class GetWallCommentsConversationRequestDataMemberNames
    {
        public const string
            MyUserId = "u",
            WallConversationId = "c",
            WallMessageId = "m";
    }
}
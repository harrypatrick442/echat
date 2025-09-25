using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatGetWallConversation)]
    public static class GetWallConversationRequestDataMemberNames
    {
        public const string
            UserId = "u",
            MyUserId = "m";
    }
}
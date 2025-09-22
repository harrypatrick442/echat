using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatGetWallConversation)]
    public static class GetWallConversationRequestDataMemberNames
    {
        public const string
            UserId = "u",
            MyUserId = "m";
    }
}
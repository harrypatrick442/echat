using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatRemoveUserFromActiveConversations)]
    public static class RemoveUserFromActiveConversationsInternalRequestDataMemberNames
    {
        public const string UserId = "i";
        public const string ConversationTypeWithConversationIds_s = "c";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatRemoveUserFromActiveConversations)]
    public static class RemoveUserFromActiveConversationsRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string UserId = "i";
        public const string ConversationIds = "c";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatDeleteMessages)]
    public static class DeleteMessagesRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string UserId = "i";
        public const string ConversationId = "c";
        public const string ConversationType = "t";
        public const string MessageIds = "m";
        public const string CanDeleteAnyMessage = "d";
    }
}
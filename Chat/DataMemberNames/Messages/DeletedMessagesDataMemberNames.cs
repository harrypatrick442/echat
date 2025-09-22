using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatDeleteMessages)]
    public static class DeletedMessagesDataMemberNames
    {
        public const string
            Ids = "p",
            ConversationId = "c";
    }
}
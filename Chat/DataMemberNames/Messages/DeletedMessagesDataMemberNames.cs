using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatDeleteMessages)]
    public static class DeletedMessagesDataMemberNames
    {
        public const string
            Ids = "p",
            ConversationId = "c";
    }
}
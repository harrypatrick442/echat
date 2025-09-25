using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatModifyMessage)]
    public static class ModifyMessageDataMemberNames
    {
        public const string ConversationId = "c";
        public const string ConversationType = "t";
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames))]
        public const string Message = "m";
    }
}
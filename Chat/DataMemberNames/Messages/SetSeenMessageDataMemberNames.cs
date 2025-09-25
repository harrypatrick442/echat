using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatSetSeenMessage)]
    public static class SetSeenMessageDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON:true)]
        public const string MyUserId = "i";
        public const string ConversationId = "c";
        public const string MessageId = "b";
    }
}
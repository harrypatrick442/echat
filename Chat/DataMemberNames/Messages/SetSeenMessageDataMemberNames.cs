using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatSetSeenMessage)]
    public static class SetSeenMessageDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON:true)]
        public const string MyUserId = "i";
        public const string ConversationId = "c";
        public const string MessageId = "b";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(MessageTypes.ChatReactToMessage)]
    public static class ReactToMessageDataMemberNames
    {
        public const string ConversationId = "c";
        public const string ConversationType = "t";
        [DataMemberNamesClass(typeof(MessageReactionDataMemberNames))]
        public const string MessageReaction = "m";
    }
}
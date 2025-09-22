using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class FetchConversationIndividualMessagesResultDataMemberNames
    {
        public const string ConversationId = "c";
        public const string ConversationType = "d";
        public const string FailedReason = "b";
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames), isArray: true)]
        public const string Messages = "i";
    }
}
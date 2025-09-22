using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class GetConversationResponseDataMemberNames
    {
        public const string
            Successful = "s",
            FailedReason = "f",
            IndexToContinueFrom = "i";
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames), isArray: true)]
        public const string Messages = "m";
        [DataMemberNamesClass(typeof(ConversationDataMemberNames), isArray: false)]
        public const string Conversation = "c";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class ReceivedMessageDataMemberNames
    {
        public const string
            ConversationId = "c",
            ConversationName = "n",
            ConversationType = "t",
            PmUserNames = "p";
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames), isArray: false)]
        public const string Message = "m";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class ConversationDataMemberNames
    {
        public const string
            IsNonVolatile = "n",
            ConversationType = "t",
            ConversationId = "c",
            ShortName = "s",
            UserIds = "u";
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames), isArray: true)]
        public const string Messages = "m";
    }
}
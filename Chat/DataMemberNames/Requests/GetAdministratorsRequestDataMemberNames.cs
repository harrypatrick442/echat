using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatGetAdministrators)]
    public static class GetAdministratorsRequestDataMemberNames
    {
        public const string
            ConversationId = "c";
        [DataMemberNamesIgnore]
        public const string
            MyUserId = "u";
    }
}
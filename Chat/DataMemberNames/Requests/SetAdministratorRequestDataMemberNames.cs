using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatSetAdministrator)]
    public static class SetAdministratorRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserId = "u",
            Privilages = "p";
        [DataMemberNamesIgnore]
        public const string
            MyUserId = "m";
    }
}
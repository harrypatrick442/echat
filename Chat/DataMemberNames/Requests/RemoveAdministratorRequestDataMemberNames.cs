using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatRemoveAdministrator)]
    public static class RemoveAdministratorRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserId = "u",
            AllowRemoveOnlyFullAdmin = "a",
            Permissions = "p";
        [DataMemberNamesIgnore]
        public const string
            MyUserId = "m";
    }
}
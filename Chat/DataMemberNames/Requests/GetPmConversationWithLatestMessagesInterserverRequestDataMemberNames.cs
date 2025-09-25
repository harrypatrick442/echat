using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatGetPmConversationWithLatestMessages)]
    public static class GetPmConversationWithLatestMessagesInterserverRequestDataMemberNames
    {
        public const string
            MyUserId = "u",
            OtherUserId = "o";
    }
}
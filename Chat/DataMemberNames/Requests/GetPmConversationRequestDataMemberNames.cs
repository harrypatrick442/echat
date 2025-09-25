using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatGetPmConversationWithLatestMessages)]
    public static class GetPmConversationRequestDataMemberNames
    {
        public const string
            OtherUserId = "o";

    }
}
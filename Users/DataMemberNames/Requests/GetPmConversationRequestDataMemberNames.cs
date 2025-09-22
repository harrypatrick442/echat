using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatGetPmConversationWithLatestMessages)]
    public static class GetPmConversationRequestDataMemberNames
    {
        public const string
            OtherUserId = "o";

    }
}
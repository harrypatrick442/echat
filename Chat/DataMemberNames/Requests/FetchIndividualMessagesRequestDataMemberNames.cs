using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatFetchIndividualMessages)]
    public static class FetchIndividualMessagesRequestDataMemberNames
    {
        public const string
            MyUserId = "c";
        [DataMemberNamesClass(typeof(ConversationAndMessageIdsDataMemberNames), isArray:true)]
        public const string
            ConversationAndMessageIds = "u";
    }
}
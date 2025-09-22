using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class FetchIndividualMessagesResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(FetchConversationIndividualMessagesResultDataMemberNames), isArray:true)]
        public const string Results = "f";
    }
}
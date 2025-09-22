using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class SearchRoomsResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(ConversationWithTagsDataMemberNames), isArray: true)]
        public const string ConversationWithTagss = "r";
    }
}
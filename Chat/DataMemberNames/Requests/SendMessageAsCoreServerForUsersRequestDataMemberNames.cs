using MessageTypes.Attributes;
using Chat.DataMemberNames.Messages;
namespace DataMemberNames.Interserver.Chat.Requests
{
    public static class SendMessageAsCoreServerForUsersRequestDataMemberNames
    {
        public const string
            UserIds = "u";
        [DataMemberNamesClass(typeof(ReceivedMessageDataMemberNames), isArray: false)]
        public const string ReceivedMessage = "r";
    }
}
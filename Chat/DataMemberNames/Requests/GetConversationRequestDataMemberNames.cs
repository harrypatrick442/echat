using DataMemberNames.Client.Users;
using MessageTypes.Attributes;
using Core.DataMemberNames;
using DataMemberNames.Client;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(ClientMessageTypes.ChatGetConversation)]
    public static class GetConversationRequestDataMemberNames
    {
        public const string
            ConversationId = "c";
    }
}
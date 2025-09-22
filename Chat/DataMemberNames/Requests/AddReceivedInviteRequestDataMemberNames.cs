using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.ChatAddReceivedInvite)]
    public static class AddReceivedInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "a",
            UserIdBeingInvited = "b",
            UserIdInviting = "c";
    }
}
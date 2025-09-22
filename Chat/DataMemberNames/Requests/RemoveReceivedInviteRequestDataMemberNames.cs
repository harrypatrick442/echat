using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.ChatRemoveReceivedInvite)]
    public static class RemoveReceivedInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserIdBeingInvited = "b",
            UserIdInviting = "i";
    }
}
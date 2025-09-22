using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.ChatRemoveSentInvite)]
    public static class RemoveSentInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserIdBeingInvited = "b",
            UserIdInviting = "i";
    }
}
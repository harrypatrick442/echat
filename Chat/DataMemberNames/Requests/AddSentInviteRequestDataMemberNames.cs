using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.ChatAddSentInvite)]
    public static class AddSentInviteRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserIdBeingInvited = "b",
            UserIdInviting = "i";
    }
}
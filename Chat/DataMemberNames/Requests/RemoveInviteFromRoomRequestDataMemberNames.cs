using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.ChatRemoveInviteFromRoom)]
    public static class RemoveInviteFromRoomRequestDataMemberNames
    {
        public const string
            ConversationId = "c",
            UserIdBeingInvited = "b",
            UserIdInviting = "u";
    }
}
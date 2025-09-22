using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.ChatRemoveUserFromRoom)]
    public static class RemoveUserFromRoomRequestDataMemberNames
    {
        public const string
            ConversationId = "c";
        public const string
            AllowRemoveOnlyFullAdmin = "a";
        [DataMemberNamesIgnore]
        public const string
            UserId = "u";
    }
}
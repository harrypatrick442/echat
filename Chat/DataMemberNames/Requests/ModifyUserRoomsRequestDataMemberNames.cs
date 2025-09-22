using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatModifyUserRooms)]
    public static class ModifyUserRoomsRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string MyUserId = "u";
        public const string ConversationId = "c";
        public const string AddElseRemove = "a";
        public const string Operations = "o";
    }
}
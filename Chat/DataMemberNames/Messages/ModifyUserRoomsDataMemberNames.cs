using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    [MessageType(global::MessageTypes.MessageTypes.ChatModifyUserRooms)]
    public static class ModifyUserRoomsDataMemberNames
    {
        public const string ConversationId = "c";
        public const string AddElseRemove = "a";
        public const string Operations = "o";
    }
}
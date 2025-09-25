using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatUpdateRoomInfo)]
    public static class UpdateChatRoomInfoRequestDataMemberNames
    {
        public const string Name = "n";
        public const string Visibility = "v";
        public const string Tags = "t";
        [DataMemberNamesIgnore(toJSON: true)]
        public const string UserId = "u";
    }
}
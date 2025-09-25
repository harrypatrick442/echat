using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatGetUserRooms)]
    public static class GetUserRoomsRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string MyUserId = "u";
        public const string Operation = "o";
    }
}
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatSearchRooms)]
    public static class SearchRoomsRequestDataMemberNames
    {
        public const string
            Str = "s";
    }
}
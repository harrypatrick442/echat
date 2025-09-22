using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatSearchRooms)]
    public static class SearchRoomsRequestDataMemberNames
    {
        public const string
            Str = "s";
    }
}
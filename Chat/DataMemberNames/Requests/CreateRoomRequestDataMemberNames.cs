using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatCreateRoom)]
    public static class CreateRoomRequestDataMemberNames
    {
        public const string Name = "n",
            Visibility = "v";
    }
}
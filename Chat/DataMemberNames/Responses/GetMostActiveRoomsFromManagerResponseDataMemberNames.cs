using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Responses
{

    [MessageType(InterserverMessageTypes.ChatGetMostActiveRooms)]
    public class GetMostActiveRoomsFromManagerResponseDataMemberNames
    {
        public const string Successful = "s";
        public const string SerializedEntries = "r";
    }
}
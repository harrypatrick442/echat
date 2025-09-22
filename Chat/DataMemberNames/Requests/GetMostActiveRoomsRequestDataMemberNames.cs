using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Requests
{

    [MessageType(InterserverMessageTypes.ChatGetMostActiveRooms)]
    public class GetMostActiveRoomsRequestDataMemberNames
    {
        public const string NMostActive = "n";
    }
}
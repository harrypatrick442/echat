using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.DataMemberNames.Responses
{

    [MessageType(InterserverMessageTypes.ChatGetMostActiveRooms)]
    public class GetMostActiveRoomsResponseDataMemberNames
    {
        public const string Successful = "s";
        [DataMemberNamesClass(typeof(RoomActivityDataMemberNames), isArray: true)]
        public const string MostActiveRooms = "r";
    }
}
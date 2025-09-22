using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetMostActiveRoomsResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetMostActiveRoomsResponseDataMemberNames.MostActiveRooms)]
        [JsonInclude]
        [DataMember(Name = GetMostActiveRoomsResponseDataMemberNames.MostActiveRooms)]
        public RoomActivity[] MostActiveRooms { get; protected set; }
        [JsonPropertyName(GetMostActiveRoomsResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetMostActiveRoomsResponseDataMemberNames.Successful)]
        public bool Successful{ get; protected set; }
        private GetMostActiveRoomsResponse(bool successful, RoomActivity[] mostActiveRooms, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            MostActiveRooms = mostActiveRooms;
            Ticket = ticket;
        }
        protected GetMostActiveRoomsResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetMostActiveRoomsResponse Success(RoomActivity[] mostActiveRooms, long ticket)
        {
            return new GetMostActiveRoomsResponse(true, mostActiveRooms, ticket);
        }
        public static GetMostActiveRoomsResponse Failed(long ticket)
        {
            return new GetMostActiveRoomsResponse(true, null, ticket);
        }
    }
}

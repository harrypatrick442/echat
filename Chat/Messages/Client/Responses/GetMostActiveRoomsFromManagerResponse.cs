using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetMostActiveRoomsFromManagerResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetMostActiveRoomsFromManagerResponseDataMemberNames.SerializedEntries)]
        [JsonInclude]
        [DataMember(Name = GetMostActiveRoomsFromManagerResponseDataMemberNames.SerializedEntries)]
        public string MostActiveRooms { get; protected set; }
        [JsonPropertyName(GetMostActiveRoomsFromManagerResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetMostActiveRoomsFromManagerResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        private GetMostActiveRoomsFromManagerResponse(bool successful, string serializedEntries, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            MostActiveRooms = serializedEntries;
            Ticket = ticket;
        }
        protected GetMostActiveRoomsFromManagerResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetMostActiveRoomsFromManagerResponse Success(string serializedEntries, long ticket)
        {
            return new GetMostActiveRoomsFromManagerResponse(true, serializedEntries, ticket);
        }
        public static GetMostActiveRoomsFromManagerResponse Failed(long ticket)
        {
            return new GetMostActiveRoomsFromManagerResponse(false, null, ticket);
        }
    }
}

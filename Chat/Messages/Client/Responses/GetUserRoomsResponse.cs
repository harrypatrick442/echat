using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetUserRoomsResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetUserRoomsResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = GetUserRoomsResponseDataMemberNames.Success)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(GetUserRoomsResponseDataMemberNames.EntriesSerialized)]
        [JsonInclude]
        [DataMember(Name = GetUserRoomsResponseDataMemberNames.EntriesSerialized)]
        public string EntriesSerialized { get; protected set; }
        protected GetUserRoomsResponse(bool successful, string entriesSerialized,
            long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            EntriesSerialized = entriesSerialized;
            _Ticket = ticket;
        }
        protected GetUserRoomsResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetUserRoomsResponse Success(string entriesSerialized, long ticket)
        {
            return new GetUserRoomsResponse(true, entriesSerialized, ticket);
        }
        public static GetUserRoomsResponse Failed(long ticket)
        {
            return new GetUserRoomsResponse(false, null, ticket);
        }
    }
}

using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetRoomSummarysResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetRoomSummarysResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetRoomSummarysResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(GetRoomSummarysResponseDataMemberNames.Summarys)]
        [JsonInclude]
        [DataMember(Name = GetRoomSummarysResponseDataMemberNames.Summarys)]
        public RoomSummary[] Summarys{ get; protected set; }
        protected GetRoomSummarysResponse(bool successful, RoomSummary[] summarys, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Summarys = summarys;
            _Ticket = ticket;
        }
        protected GetRoomSummarysResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetRoomSummarysResponse Success(RoomSummary[] summarys, long ticket)
        {
            return new GetRoomSummarysResponse(true, summarys, ticket);
        }
        public static GetRoomSummarysResponse Failed(long ticket)
        {
            return new GetRoomSummarysResponse(false, null, ticket);
        }
    }
}

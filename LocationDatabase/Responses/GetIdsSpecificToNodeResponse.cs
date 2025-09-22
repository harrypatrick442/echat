using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using LocationDatabase;
using LocationCore;
using Location.DataMemberNames.Interserver.Responses;

namespace Location.Responses
{
    [DataContract]
    public class GetIdsSpecificToNodeResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetIdsSpecificToNodeResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetIdsSpecificToNodeResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(GetIdsSpecificToNodeResponseDataMemberNames.Quadrants)]
        [JsonInclude]
        [DataMember(Name = GetIdsSpecificToNodeResponseDataMemberNames.Quadrants)]
        public Quadrant[] Quadrants{ get; protected set; }
        public GetIdsSpecificToNodeResponse(bool successful, Quadrant[] quadrants, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Quadrants = quadrants;
            Successful = successful;
            Ticket = ticket;
        }
        protected GetIdsSpecificToNodeResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetIdsSpecificToNodeResponse Success(Quadrant[] quadrants, long ticket)
        {
            return new GetIdsSpecificToNodeResponse(true, quadrants, ticket);
        }
        public static GetIdsSpecificToNodeResponse Failure(long ticket)
        {
            return new GetIdsSpecificToNodeResponse(false, null, ticket);
        }
    }
}

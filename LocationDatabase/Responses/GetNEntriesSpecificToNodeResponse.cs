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
    public class GetNEntriesSpecificToNodeResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetNEntriesSpecificToNodeResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetNEntriesSpecificToNodeResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(GetNEntriesSpecificToNodeResponseDataMemberNames.QuadrantNEntriess)]
        [JsonInclude]
        [DataMember(Name = GetNEntriesSpecificToNodeResponseDataMemberNames.QuadrantNEntriess)]
        public QuadrantNEntries[] QuadrantNEntriess{ get; protected set; }
        public GetNEntriesSpecificToNodeResponse(bool successful, QuadrantNEntries[] quadrantNEntriess, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            QuadrantNEntriess = quadrantNEntriess;
            Successful = successful;
            Ticket = ticket;
        }
        protected GetNEntriesSpecificToNodeResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetNEntriesSpecificToNodeResponse Success(QuadrantNEntries[] quadrantNEntriess, long ticket)
        {
            return new GetNEntriesSpecificToNodeResponse(true, quadrantNEntriess, ticket);
        }
        public static GetNEntriesSpecificToNodeResponse Failure(long ticket)
        {
            return new GetNEntriesSpecificToNodeResponse(false, null, ticket);
        }
    }
}

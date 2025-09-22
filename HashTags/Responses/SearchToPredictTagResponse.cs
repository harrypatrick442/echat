using Core.DataMemberNames;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Responses;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HashTags.Messages
{
    [DataContract]
    public class SearchToPredictTagResponse : TicketedMessageBase
    {
        [JsonPropertyName(SearchToPredictTagResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = SearchToPredictTagResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(SearchToPredictTagResponseDataMemberNames.Matches)]
        [JsonInclude]
        [DataMember(Name = SearchToPredictTagResponseDataMemberNames.Matches)]
        public string[]? Matches{ get; protected set; }
        public SearchToPredictTagResponse(bool success, string[]? matches, long ticket)
            :base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Matches = matches;
            Ticket = ticket;
        }
        protected SearchToPredictTagResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

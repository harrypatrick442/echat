using Core.DataMemberNames;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Responses;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HashTags.Messages
{
    [DataContract]
    public class SearchTagsResponse:TicketedMessageBase
    {
        [JsonPropertyName(SearchTagsResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = SearchTagsResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(SearchTagsResponseDataMemberNames.ExactMatches)]
        [JsonInclude]
        [DataMember(Name = SearchTagsResponseDataMemberNames.ExactMatches)]
        public ScopeIds[]? ExactMatches{ get; protected set; }
        [JsonPropertyName(SearchTagsResponseDataMemberNames.PartialMatches)]
        [JsonInclude]
        [DataMember(Name = SearchTagsResponseDataMemberNames.PartialMatches)]
        public TagWithScopeIds[]? PartialMatches { get; protected set; }
        public SearchTagsResponse(bool success, 
            ScopeIds[]? exactMatches, TagWithScopeIds[]? partialMatches, long ticket)
            :base(TicketedMessageType.Ticketed)
        {
            Success = success;
            ExactMatches = exactMatches;
            PartialMatches = partialMatches;
            Ticket = ticket;
        }
        protected SearchTagsResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

using Core.DataMemberNames;
using Core.Messages.Messages;
using HashTags.DataMemberNames.Responses;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HashTags.Messages
{
    [DataContract]
    public class SearchTagsMultipleScopeTypesResponse : TicketedMessageBase
    {
        [JsonPropertyName(SearchTagsMultipleScopeTypesResponseDataMemberNames.Results)]
        [JsonInclude]
        [DataMember(Name = SearchTagsMultipleScopeTypesResponseDataMemberNames.Results)]
        public SearchTagsResultForScopeType[] Results { get; set; }
        public SearchTagsMultipleScopeTypesResponse(
            SearchTagsResultForScopeType[] results, long ticket)
            :base(TicketedMessageType.Ticketed)
        {
            Results = results;
            Ticket = ticket;
        }
        protected SearchTagsMultipleScopeTypesResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

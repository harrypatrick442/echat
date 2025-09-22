using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Core.Messages.Messages;
using Users.DataMemberNames.Responses;

namespace Users.Messages.Client
{
    [DataContract]
    public class UsernameSearchSearchResponse : TicketedMessageBase
    {
        [JsonPropertyName(UsernameSearchSearchResponseDataMemberNames.UserIds)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchSearchResponseDataMemberNames.UserIds)]
        public long[] UserIds { get; protected set; }
        [JsonPropertyName(UsernameSearchSearchResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchSearchResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public UsernameSearchSearchResponse(long[] userIds, bool success, long ticket) : 
            base(TicketedMessageType.Ticketed)
        {
            UserIds = userIds;
            Success = success;
            Ticket = ticket;
        }
        protected UsernameSearchSearchResponse() :
            base(TicketedMessageType.Ticketed)
        { }
    }
}

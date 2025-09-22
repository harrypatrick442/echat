using Core.Messages.Messages;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Responses;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetPublicUserProfileSummarysResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetPublicUserProfileSummarysResponseDataMemberNames.UserProfileSummarys)]
        [JsonInclude]
        [DataMember(Name = GetPublicUserProfileSummarysResponseDataMemberNames.UserProfileSummarys)]
        public UserProfileSummary[] UserProfileSummarys
        {
            get;
            protected set;
        }
        public GetPublicUserProfileSummarysResponse(
            UserProfileSummary[] userProfileSummarys, long ticket) : base(TicketedMessageType.Ticketed)
        {
            UserProfileSummarys = userProfileSummarys;
            _Ticket = ticket;
        }
        protected GetPublicUserProfileSummarysResponse() : base(TicketedMessageType.Ticketed)
        { }
    }
}

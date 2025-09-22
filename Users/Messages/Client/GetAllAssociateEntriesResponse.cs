using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;
using Users.DataMemberNames.Responses;

namespace Users.Messages.Client
{
    [DataContract]
    public class GetAllAssociateEntriesResponse:TicketedMessageBase
    {
        [JsonPropertyName(GetAllAssociateEntriesResponseDataMemberNames.MyAssociates)]
        [JsonInclude]
        [DataMember(Name = GetAllAssociateEntriesResponseDataMemberNames.MyAssociates)]

        public UserProfileSummary[] MyAssociates { get; protected set; }
        [JsonPropertyName(GetAllAssociateEntriesResponseDataMemberNames.ReceivedRequests)]
        [JsonInclude]
        [DataMember(Name = GetAllAssociateEntriesResponseDataMemberNames.ReceivedRequests)]

        public AssociateRequestUserProfileSummarys ReceivedRequests { get; protected set; }
        [JsonPropertyName(GetAllAssociateEntriesResponseDataMemberNames.SentRequests)]
        [JsonInclude]
        [DataMember(Name = GetAllAssociateEntriesResponseDataMemberNames.SentRequests)]

        public AssociateRequestUserProfileSummarys SentRequests { get; protected set; }
        public GetAllAssociateEntriesResponse(UserProfileSummary[] associates, AssociateRequestUserProfileSummarys receivedRequests,
            AssociateRequestUserProfileSummarys sentRequests, long ticket):base(TicketedMessageType.Ticketed) {
            MyAssociates = associates;
            ReceivedRequests = receivedRequests;
            SentRequests = sentRequests;
            _Ticket = ticket;
        }
        protected GetAllAssociateEntriesResponse() : base(TicketedMessageType.Ticketed) {
            
        }
    }
}

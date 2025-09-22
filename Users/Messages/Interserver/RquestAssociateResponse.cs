using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;
using Core.Messages.Responses;
using Users.DataMemberNames.Responses;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class RquestAssociateResponse : SuccessTicketedResponse
    {
        [JsonPropertyName(RquestAssociateResponseDataMemberNames.ActingAssociateRequestUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = RquestAssociateResponseDataMemberNames.ActingAssociateRequestUserProfileSummary)]
        public AssociateRequestUserProfileSummary ActingAssociateRequestUserProfileSummary
        {
            get;
            protected set;
        }
        [JsonPropertyName(RquestAssociateResponseDataMemberNames.OtherUserAssociateRequestUserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = RquestAssociateResponseDataMemberNames.OtherUserAssociateRequestUserProfileSummary)]
        public AssociateRequestUserProfileSummary OtherUserAssociateRequestUserProfileSummary
        {
            get;
            protected set;
        }
        public RquestAssociateResponse(bool success, 
            AssociateRequestUserProfileSummary actingAssociateRequestUserProfileSummary,
            AssociateRequestUserProfileSummary otherUserAssociateRequestUserProfileSummary, 
            long ticket)
            : base(success, ticket)
        {
            ActingAssociateRequestUserProfileSummary = actingAssociateRequestUserProfileSummary;
            OtherUserAssociateRequestUserProfileSummary = otherUserAssociateRequestUserProfileSummary;
        }
        protected RquestAssociateResponse()
        { }
    }
}

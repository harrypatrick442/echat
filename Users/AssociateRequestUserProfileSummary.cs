using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Requests;

namespace Users
{
    [DataContract]
    public class AssociateRequestUserProfileSummary
    {
        private AssociateRequest _AssociateRequest;
        [JsonPropertyName(AssociateRquestUserProfileSummaryDataMemberNames.AssociateRequest)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestUserProfileSummaryDataMemberNames.AssociateRequest)]
        public AssociateRequest AssociateRequest
        {
            get { return _AssociateRequest; }
            protected set { _AssociateRequest = value; }
        }
        [JsonPropertyName(AssociateRquestUserProfileSummaryDataMemberNames.UserProfileSummary)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestUserProfileSummaryDataMemberNames.UserProfileSummary)]
        public UserProfileSummary UserProfileSummary
        {
            get;
            protected set;
        }
        public AssociateRequestUserProfileSummary(AssociateRequest associateRequest,
            UserProfileSummary userProfileSummary)
        {
            _AssociateRequest = associateRequest;
            UserProfileSummary = userProfileSummary;
        }
        protected AssociateRequestUserProfileSummary() { }
    }
}

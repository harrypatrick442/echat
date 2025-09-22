using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Users.DataMemberNames.Responses;

namespace Users.Messages.Client
{
    [DataContract]
    public class GetMyAssociatesProfileSummariesResponse {
        private UserProfileSummary[] _UserProfileSummarys;
        [JsonPropertyName(GetMyAssociatesProfileSummariesResponseDataMemberNames.UserProfileSummarys)]
        [JsonInclude]
        [DataMember(Name = GetMyAssociatesProfileSummariesResponseDataMemberNames.UserProfileSummarys)]

        public UserProfileSummary[] UserProfileSummarys { get { return _UserProfileSummarys; } protected set { _UserProfileSummarys = value; } }
        public GetMyAssociatesProfileSummariesResponse(UserProfileSummary[] userProfileSummarys) {
            _UserProfileSummarys = userProfileSummarys;
        }
        protected GetMyAssociatesProfileSummariesResponse() { }
    }
}

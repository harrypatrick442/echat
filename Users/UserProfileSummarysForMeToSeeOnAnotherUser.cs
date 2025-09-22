using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Messages;

namespace Users
{
    [DataContract]
    public class UserProfileSummarysForMeToSeeOnAnotherUser
    {
        private AssociatesFailedReason? _FailedReason;
        [JsonPropertyName(UserProfileSummarysForMeToSeeOnAnotherUserDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummarysForMeToSeeOnAnotherUserDataMemberNames.FailedReason)]
        public AssociatesFailedReason? FailedReason { get { return _FailedReason; } protected set { _FailedReason = value; } }
        private UserProfileSummary[] _UserProfileSummarys;
        [JsonPropertyName(UserProfileSummarysForMeToSeeOnAnotherUserDataMemberNames.UserProfileSummarys)]
        [JsonInclude]
        [DataMember(Name = UserProfileSummarysForMeToSeeOnAnotherUserDataMemberNames.UserProfileSummarys)]
        public UserProfileSummary[] UserProfileSummarys  { get { return _UserProfileSummarys; } protected set { _UserProfileSummarys = value; } }
        private UserProfileSummarysForMeToSeeOnAnotherUser(AssociatesFailedReason failedReason) {
            _FailedReason = failedReason;
        }
        public UserProfileSummarysForMeToSeeOnAnotherUser(UserProfileSummary[] userProfileSummarys)
        {
            _UserProfileSummarys = userProfileSummarys;
        }
        protected UserProfileSummarysForMeToSeeOnAnotherUser() { }
        public static UserProfileSummarysForMeToSeeOnAnotherUser NotVisible()
        {
            return new UserProfileSummarysForMeToSeeOnAnotherUser(AssociatesFailedReason.NotVisible);
        }
    }
}

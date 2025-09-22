using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Messages;

namespace Users
{
    [DataContract]
    public class UserProfileForMeToSeeOnAnotherUser
    {
        private AssociatesFailedReason? _FailedReason;
        [JsonPropertyName(UserProfileForMeToSeeOnAnotherUserDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = UserProfileForMeToSeeOnAnotherUserDataMemberNames.FailedReason)]
        public AssociatesFailedReason? FailedReason { get { return _FailedReason; } protected set { _FailedReason = value; } }
        private UserProfile _UserProfile;
        [JsonPropertyName(UserProfileForMeToSeeOnAnotherUserDataMemberNames.UserProfile)]
        [JsonInclude]
        [DataMember(Name = UserProfileForMeToSeeOnAnotherUserDataMemberNames.UserProfile)]
        public UserProfile UserProfile  { get { return _UserProfile; } protected set { _UserProfile = value; } }
        private UserProfileForMeToSeeOnAnotherUser(AssociatesFailedReason failedReason) {
            _FailedReason = failedReason;
        }
        public UserProfileForMeToSeeOnAnotherUser(UserProfile userProfile)
        {
            _UserProfile = userProfile;
        }
        protected UserProfileForMeToSeeOnAnotherUser() { }
        public static UserProfileForMeToSeeOnAnotherUser NotVisible()
        {
            return new UserProfileForMeToSeeOnAnotherUser(AssociatesFailedReason.NotVisible);
        }
    }
}

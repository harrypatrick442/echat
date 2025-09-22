using MessageTypes.Attributes;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users.DataMemberNames.Messages
{
    public static class UserProfileForMeToSeeOnAnotherUserDataMemberNames
    {
        public const string FailedReason = "f";

        [DataMemberNamesClass(typeof(UserProfileDataMemberNames), isArray: false)]
        public const string UserProfile = "u";
    }
}

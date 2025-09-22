
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Attributes;

namespace Users.DataMemberNames.Messages
{
    public class UserProfileSummarysForMeToSeeOnAnotherUserDataMemberNames
    {
        public const string FailedReason = "f";
        [DataMemberNamesClass(typeof(UserProfileSummaryDataMemberNames), isArray: true)]
        public const string UserProfileSummarys = "u";
    }
}

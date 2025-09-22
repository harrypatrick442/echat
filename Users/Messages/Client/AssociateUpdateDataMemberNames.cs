using MessageTypes.Attributes;
using Users.DataMemberNames.Requests;

namespace Users.DataMemberNames.Messages
{
    public class AssociateUpdateDataMemberNames
    {
        public const string ActingUserId = "u", Operation = "v", OtherUserId = "t",
            AssociateType = "a";
        [DataMemberNamesClass(typeof(AssociateRquestUserProfileSummaryDataMemberNames))]
        public const string ActingAssociateRequestUserProfileSummary = "r";
        [DataMemberNamesClass(typeof(AssociateRquestUserProfileSummaryDataMemberNames))]
        public const string OtherUserAssociateRequestUserProfileSummary = "o";
        [DataMemberNamesClass(typeof(UserProfileSummaryDataMemberNames))]
        public const string ActingUserProfileSummary = "w";
        [DataMemberNamesClass(typeof(UserProfileSummaryDataMemberNames))]
        public const string OtherUserProfileSummary="x";
    }
}
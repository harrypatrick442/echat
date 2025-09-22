using Users.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    public static class AssociateRquestUserProfileSummaryDataMemberNames
    {
        [DataMemberNamesClass(typeof(AssociateRquestDataMemberNames), isArray: false)]
        public const string AssociateRequest = "a";

        [DataMemberNamesClass(typeof(UserProfileSummaryDataMemberNames), isArray: false)]
        public const string UserProfileSummary = "p";
    }
}
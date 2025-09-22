using MessageTypes.Attributes;
using Users.DataMemberNames.Messages;
using Users.DataMemberNames.Requests;

namespace Users.DataMemberNames.Responses
{
    public static class GetAllAssociateEntriesResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(UserProfileSummaryDataMemberNames), isArray: true)]
        public const string
            MyAssociates = "a";
        [DataMemberNamesClass(typeof(AssociateRquestUserProfileSummarysDataMemberNames), isArray: false)]
        public const string
            ReceivedRequests = "r";
        [DataMemberNamesClass(typeof(AssociateRquestUserProfileSummarysDataMemberNames), isArray: false)]
        public const string
            SentRequests = "s";

    }
}
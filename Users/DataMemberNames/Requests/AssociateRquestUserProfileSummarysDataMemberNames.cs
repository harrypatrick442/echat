using MessageTypes.Attributes;

namespace Users.DataMemberNames.Requests
{
    public static class AssociateRquestUserProfileSummarysDataMemberNames
    {
        [DataMemberNamesClass(typeof(AssociateRquestUserProfileSummaryDataMemberNames), isArray: true)]
        public const string
            Entries = "e";
    }
}
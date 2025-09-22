using MessageTypes.Attributes;
using Users.DataMemberNames.Messages;

namespace Users.DataMemberNames.Responses
{
    public static class GetPublicUserProfileSummarysResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(UserProfileSummaryDataMemberNames), isArray: true)]
        public const string UserProfileSummarys = "u";
    }
}
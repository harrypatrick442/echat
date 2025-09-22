using MessageTypes.Attributes;
using Users.DataMemberNames.Messages;

namespace Users.DataMemberNames.Responses
{
    public static class UsernameSearchSearchResponseDataMemberNames
    {
        public const string Success = "s";
        public const string UserIds = "u";
        [DataMemberNamesClass(typeof(UserProfileSummaryDataMemberNames), isArray: true)]
        public const string UserProfileSummarys = "p";
    }
}
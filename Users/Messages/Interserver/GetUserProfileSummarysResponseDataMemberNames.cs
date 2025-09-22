using MessageTypes.Attributes;
using Users.DataMemberNames.Messages;

namespace Users.Messages.Interserver
{
    public static class GetUserProfileSummarysResponseDataMemberNames
    {
        public const string
            UserIdsCouldNotGet = "c";
        [DataMemberNamesClassAttribute(typeof(UserProfileSummaryDataMemberNames), isArray: true)]
        public const string UserProfileSummarys = "a";
    }
}
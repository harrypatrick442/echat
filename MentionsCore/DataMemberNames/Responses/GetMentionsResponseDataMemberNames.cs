using MentionsCore.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace MentionsCore.DataMemberNames.Responses
{
    public static class GetMentionsResponseDataMemberNames
    {
        public const string
            Successful = "a";
        [DataMemberNamesClass(typeof(MentionDataMemberNames), isArray: true)]
        public const string
            Entries = "b";
    }
}
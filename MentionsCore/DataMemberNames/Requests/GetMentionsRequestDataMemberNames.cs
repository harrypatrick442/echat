using MessageTypes.Attributes;

namespace MentionsCore.DataMemberNames.Requests
{
    [MessageType(MessageTypes.MentionsGet)]
    public static class GetMentionsRequestDataMemberNames
    {
        public const string
            UserId = "a",
            IdToExclusive = "b",
            NEntries = "c",
            IdFromInclusive = "d";
    }
}
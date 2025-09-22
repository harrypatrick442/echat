using MessageTypes.Attributes;

namespace MentionsCore.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.MentionsGet)]
    public static class GetMentionsRequestDataMemberNames
    {
        public const string
            UserId = "a",
            IdToExclusive = "b",
            NEntries = "c",
            IdFromInclusive = "d";
    }
}
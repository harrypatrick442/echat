using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatGetConversationSnapshots)]
    public static class GetConversationSnapshotsRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string
            MyUserId = "i";
        public const string
            IdFromInclusive = "m",
            IdToExclusive = "n",
            NEntries = "o";
    }
}
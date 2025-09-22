using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class UserConversationSnapshotsDataMemberNames
    {
        public const string UserId = "u";
        public const string WroteEntriesToCurrentHistory = "w";
        public const string HistoryIndex = "h";
        [DataMemberNamesClass(typeof(ConversationSnapshotDataMemberNames), isArray: true)]
        public const string ConversationSnapshots = "c";
    }
}
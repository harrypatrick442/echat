using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class OlderConversationSnapshotsDataMemberNames
    {
        [DataMemberNamesClass(typeof(ConversationSnapshotDataMemberNames), isArray: true)]
        public const string Entries = "e";
        //public const string PreviousConversationSnapshotsId="p";
    }
}
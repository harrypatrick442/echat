using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class GetMyConversationSnapshotsResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(ConversationSnapshotDataMemberNames),
            isArray: true)]
        public const string ConversationSnapshots = "u";
    }
}
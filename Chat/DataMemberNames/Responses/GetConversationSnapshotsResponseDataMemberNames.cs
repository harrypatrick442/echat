using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class GetConversationSnapshotsResponseDataMemberNames
    {
        public const string
            Successful = "s",
            FailedReason = "f";
        //t DO NOT USE
        [DataMemberNamesClass(typeof(ConversationSnapshotDataMemberNames), isArray: true)]
        public const string Entries = "m";
        //TODO remove or implement
        [DataMemberNamesClass(typeof(MessageReactionDataMemberNames), isArray: true)]
        public const string Reactions = "r";
        [DataMemberNamesClass(typeof(MessageUserMultimediaItemDataMemberNames), isArray: true)]
        public const string UserMultimediaItems = "w";
    }
}
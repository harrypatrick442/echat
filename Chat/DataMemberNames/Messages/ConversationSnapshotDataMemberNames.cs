using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public class ConversationSnapshotDataMemberNames : ClientMessageDataMemberNames
    {
        public const string
            UserIdsInConversation = "o",
            Seen = "y";
    }
}
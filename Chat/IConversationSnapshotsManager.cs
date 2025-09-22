using Chat.Messages.Client.Messages;

namespace Chat
{
    public interface IConversationSnapshotsManager
    {
        ConversationSnapshot[] GetLatest(long userId);
        void UpdateLatestMessage_Here(long myUserId,
            ClientMessage receivedMessage, long[] userIdsInConversation);
    }
}
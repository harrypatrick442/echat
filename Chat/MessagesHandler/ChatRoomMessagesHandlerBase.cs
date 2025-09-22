using Chat.Interfaces;
using Chat.Messages.Client;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;

namespace Chat
{
    public abstract class ChatRoomMessagesHandlerBase
    {
        protected  LatestCachedMessages _LatestCachedMessages;
        public ChatRoomMessagesHandlerBase(int maxNMessages)
        {
            _LatestCachedMessages = new LatestCachedMessages(maxNMessages, FlushMessages);
        }
        public abstract void Add(ClientMessage message, out string jsonString);
        public abstract void ReactToMessage(ReactToMessage reactToMessage);
        public abstract void UnreactToMessage(UnreactToMessage unreactToMessage);
        public abstract long[] DeleteMessages(long messageUserId, long[] messageIds,
            bool canDeleteAnyMessage);
        public abstract bool ModifyMessage(ModifyMessage modifyMessageRequest);
        public abstract void FlushMessagesIfChanged();
        protected abstract void FlushMessages(ClientMessage[] messages, MessageReaction[] reactions, MessageUserMultimediaItem[] userMultimediaItems);
        public string GetMessagesJSONArrayString(out string reactionsJSONArrayString,
            out string userMultimediaItemsJsonArrayString)
        {
            return _LatestCachedMessages.GetJSONArrayString(out reactionsJSONArrayString, out userMultimediaItemsJsonArrayString);
        }
        public abstract void LoadMessagesHistory(
            long? idFromInclusive, long? idToExclusive, int? nEntries, out ClientMessage[] messages,
            out MessageReaction[] reactions, out MessageUserMultimediaItem[] userMultimediaItems);
    }
}
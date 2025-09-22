using Core.Exceptions;
using KeyValuePairDatabases;
using Chat.Messages.Client.Messages;
using Core.DAL;
using Chat.Messages.Client;

namespace Chat
{
    public class ConversationSnapshotsManager
    {
        private static ConversationSnapshotsManager _Instance;
        public static ConversationSnapshotsManager Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(ConversationSnapshotsManager));
            _Instance = new ConversationSnapshotsManager();
            return _Instance;
        }
        public static ConversationSnapshotsManager Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(ConversationSnapshotsManager));
                return _Instance;
            }
        }
        private KeyValuePairInMemoryDatabase<long, CachedUserConversationSnapshots> _Cache;
        private ConversationSnapshotsManager()
        {
            _Cache = new KeyValuePairInMemoryDatabase<
                long, CachedUserConversationSnapshots>(
                new OverflowParameters<long, CachedUserConversationSnapshots> (
                    overflowToNowhere:true,
                    new NoIdentifierLock<long>(),
                    (identifier, entry) => { }
                    ),
                new IdentifierLock<long>());

        }
        #region Methods
        #region Public
        public ConversationSnapshot[] GetUserConversationSnapshotsLocal(long userId,
            long? idFromInclusive, long? idToExclusive, int? nEntries, out MessageReaction[] reactions,
            out MessageUserMultimediaItem[] messageUserMultimediaItems)
        {
            reactions = null;
            messageUserMultimediaItems = null;
            ConversationSnapshot[] conversationSnapshots = null;
            CachedUserConversationSnapshots cached =  _Cache.Get(userId);
            if (cached == null)
            {
                if ((idFromInclusive == null && idToExclusive == null))
                {
                    _Cache.ReadCallbackWriteWithinLock(userId, (cachedInternal) =>
                    {
                        if (cachedInternal != null)
                        {
                            conversationSnapshots = cachedInternal.ConversationSnapshots;
                            return cachedInternal;
                        }
                        conversationSnapshots = DalConversationSnapshots.Instance.ReadRange(
                            userId, ChatConstants.CONVERSATION_SNAPSHOTS_N_ENTRIES_CACHE, null, null);
                        return new CachedUserConversationSnapshots(conversationSnapshots);
                    });
                    return conversationSnapshots;
                }
            }
            else {
                if ((idFromInclusive==null&&idToExclusive==null)||idFromInclusive >= cached.IdFromInclusive) {
                    conversationSnapshots = cached.ConversationSnapshots;
                    return conversationSnapshots;
                }
            }
            conversationSnapshots = DalConversationSnapshots.Instance.ReadRange(
                userId, nEntries, idFromInclusive, idToExclusive);
            return conversationSnapshots;
        }
        public void UpdateLatestMessage_Here(long myUserId, 
                ClientMessage receivedMessage, long[] userIdsInConversation)
        {
            // if (userId == receivedMessage.UserId) return;
            //LOCKUP HAPPENED IN THIS BIT OF CODE.
            _Cache.ReadCallbackWriteWithinLock(myUserId, cachedUserConversationSnapshots =>
            {
                if (cachedUserConversationSnapshots == null)
                {
                    cachedUserConversationSnapshots = new CachedUserConversationSnapshots();
                }
                ConversationSnapshot converationSnapshot = 
                cachedUserConversationSnapshots.UpdateLatestMessageOrAdd(receivedMessage, userIdsInConversation);
                DalConversationSnapshots.Instance.InsertOrReplace(
                    myUserId, (long)receivedMessage.ConversationId, converationSnapshot);
                return cachedUserConversationSnapshots;
            });
        }
        public void SetSeenMessage_Here(long myUserId, long conversationId, long messageId)
        {
            _Cache.ReadCallbackWriteWithinLock(myUserId, cachedUserConversationSnapshots =>
            {
                if (cachedUserConversationSnapshots != null)
                {
                    cachedUserConversationSnapshots.SetSeen(conversationId, messageId);
                }
                DalConversationSnapshots.Instance.SetSeen(
                    myUserId, conversationId, messageId);
                return cachedUserConversationSnapshots;
            });
        }
        #endregion Public
        #region Private
        #endregion Private
        #endregion Methods
    }
}
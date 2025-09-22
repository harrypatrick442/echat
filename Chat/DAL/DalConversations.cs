using Core.Enums;
using Core.Exceptions;
using Chat;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace Core.DAL
{
    public class DalConversations
    {
        private static DalConversations _Instance;
        public static DalConversations Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalConversations));
                return _Instance;            }
        }
        public static DalConversations Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalConversations));
                _Instance = new DalConversations();
                return _Instance;
        }
        private KeyValuePairDatabaseMesh<long, Conversation> _ConversationIdToConversationKeyValuePairDatabase;
        private DalConversations()
        {
            _ConversationIdToConversationKeyValuePairDatabase
            = new KeyValuePairDatabaseMesh<long, Conversation>(
                DatabaseIdentifier.ConversationIdToConversation.Int(),
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.ConversationIdToConversationsDatabaseDirectory),
                    NCharactersEachLevel = 2
                }, new IdentifierLock<long>(),
                ConversationIdToNodeId.Instance);
        }/*
        public UserConversationSnapshots GetUserConversationSnapshots(long myUserId)
        {
            return _UserIdToUserConversationSnapshotsKeyValuePairDatabase.Get(myUserId);
        }
        public void ModifyUserConversationSnapshots(long userId, Func<UserConversationSnapshots, UserConversationSnapshots> callback)
        {
            _UserIdToUserConversationSnapshotsKeyValuePairDatabase.ModifyWithinLock(userId, callback);
        }*/
        public Conversation CreateConversation(
            ConversationType conversationType, long[] userIds, string shortName, bool isNonVolatile = true)
        {
            Conversation conversation = new Conversation(
                ConversationIdSource.Instance.NextId(),
                conversationType, userIds, shortName, isNonVolatile);
            _ConversationIdToConversationKeyValuePairDatabase.Set(conversation.ConversationId, conversation);
            return conversation;
        }
        public Conversation CreateConversation(
            ConversationType conversationType, long[] userIds, bool isNonVolatile = true)
        {
            Conversation conversation = new Conversation(
                ConversationIdSource.Instance.NextId(),
                conversationType, userIds, isNonVolatile);
            _ConversationIdToConversationKeyValuePairDatabase.Set(conversation.ConversationId, conversation);
            return conversation;
        }
        public Conversation GetConversation(long conversationId)
        {
            return _ConversationIdToConversationKeyValuePairDatabase.Get(conversationId);
        }
        public void ModifyConversation(long conversationId, Func<Conversation, Conversation> callback)
        {
            _ConversationIdToConversationKeyValuePairDatabase.ModifyWithinLock(conversationId, callback);
        }
    }
}
using Core.Enums;
using Core.Exceptions;
using Chat;
using KeyValuePairDatabases;
using Core.Ids;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace Core.DAL
{
    public class DalComments
    {
        private static DalComments _Instance;
        public static DalComments Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalComments));
                return _Instance;
            }
        }
        public static DalComments Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalComments));
                _Instance = new DalComments();
                return _Instance;
        }

        private KeyValuePairDatabase<long,  Comments> _UserIdToCommentsKeyValuePairDatabase;
        private DalComments()
        {
            _UserIdToCommentsKeyValuePairDatabase
            = new KeyValuePairDatabase<long, Comments>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(
                        DependencyNames.ConversationIdToCommentsDatabaseDirectory)
                }, 
                new IdentifierLock<long>());
        }
        public Comments GetCommentsByConversationId(long conversationId)
        {
            return _UserIdToCommentsKeyValuePairDatabase.Get(conversationId);
        }
        public Comments GetIfInMemory(long conversationId)
        {
            return _UserIdToCommentsKeyValuePairDatabase.GetIfInMemory(conversationId);
        }
        public void SetComments(long conversationId, Comments comments) {
            _UserIdToCommentsKeyValuePairDatabase.Set(conversationId, comments, true);
        }
        /*
        public void ModifyComments(long conversationId, 
            Action<Comments, Action<Comments>> callback) {
            _UserIdToCommentsKeyValuePairDatabase.ModifyWithinLockWithSet(conversationId,
                callback
            );
        }*/
    }
}
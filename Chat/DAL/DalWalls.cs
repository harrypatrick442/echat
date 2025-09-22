using Core.Enums;
using Core.Exceptions;
using Chat;
using KeyValuePairDatabases;
using Core.Ids;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace Core.DAL
{
    public class DalWalls
    {
        private static DalWalls _Instance;
        public static DalWalls Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalWalls));
                return _Instance;
            }
        }
        public static DalWalls Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalWalls));
                _Instance = new DalWalls();
                return _Instance;
        }

        private KeyValuePairDatabase<long,  Wall> _UserIdToWallKeyValuePairDatabase;
        private DalWalls()
        {
            _UserIdToWallKeyValuePairDatabase
            = new KeyValuePairDatabase<long, Wall>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.ConversationIdToWallDatabaseDirectory)
                }, 
                new IdentifierLock<long>());
        }
        public Wall GetWallByConversationId(long conversationId)
        {
            return _UserIdToWallKeyValuePairDatabase.Get(conversationId);
        }
        public Wall GetIfInMemory(long conversationId)
        {
            return _UserIdToWallKeyValuePairDatabase.GetIfInMemory(conversationId);
        }
        public void SetWall(long conversationId, Wall wall) {
            _UserIdToWallKeyValuePairDatabase.Set(conversationId, wall, true);
        }
        public void ModifyWall(long conversationId, 
            Action<Wall, Action<Wall>> callback) {
            _UserIdToWallKeyValuePairDatabase.ModifyWithinLockWithSet(conversationId,
                callback
            );
        }
    }
}
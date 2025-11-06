using KeyValuePairDatabases;
using Core.Enums;
using Core.Exceptions;
using KeyValuePairDatabases.Enums;
using DependencyManagement;
using Initialization.Exceptions;

namespace Users.DAL
{
    public class DalAssociates
    {
        private static DalAssociates _Instance;
        public static DalAssociates Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalAssociates));
                return _Instance;
            }
        }
        public static DalAssociates Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalAssociates));
                _Instance = new DalAssociates();
                return _Instance;
        }

        private KeyValuePairDatabaseMesh<long, Associates> _UserIdToAssociatesKeyValuePairDatabase;
        private DalAssociates()
        {
            _UserIdToAssociatesKeyValuePairDatabase
            = new KeyValuePairDatabaseMesh<long, Associates>(
                DatabaseIdentifier.UserIdToAssociates.Int(), 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToAssociatesDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension= ".json" 
                }, new IdentifierLock<long>(), 
                UserIdToNodeId.Instance);
        }
        public Associates GetUsersAssociates(long myUserId)
        {
            return _UserIdToAssociatesKeyValuePairDatabase.Get(myUserId);
        }
        public void ModifyAssociates(long userId, Func<Associates, Associates> callback) {
            _UserIdToAssociatesKeyValuePairDatabase.ModifyWithinLock(userId, (associates) => {
                if (associates == null) associates = new Associates();
                return callback(associates);
            });
        }
    }
}
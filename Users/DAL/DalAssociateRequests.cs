using Core.Enums;
using Core.Exceptions;
using DependencyManagement;
using Initialization.Exceptions;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;

namespace Users.DAL
{
    public class DalAssociateRequests
    {
        private static DalAssociateRequests _Instance;
        public static DalAssociateRequests Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalAssociateRequests));
                return _Instance;
            }
        }
        public static DalAssociateRequests Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalAssociateRequests));
                _Instance = new DalAssociateRequests();
                return _Instance;
        }

        private KeyValuePairDatabaseMesh<long, AssociateRequestsSent> _UserIdToAssociateRequestsSentKeyValuePairDatabase;
        private KeyValuePairDatabaseMesh<long, AssociateRequestsReceived> _UserIdToAssociateRequestsReceivedKeyValuePairDatabase;
        private DalAssociateRequests()
        {
            _UserIdToAssociateRequestsSentKeyValuePairDatabase
            = new KeyValuePairDatabaseMesh<long, AssociateRequestsSent>(
                DatabaseIdentifier.UserIdToAssociateRequestsSent.Int(),
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToAssociateRequestsSentDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension = ".json"
                }, new IdentifierLock<long>(), UserIdToNodeId.Instance);


            _UserIdToAssociateRequestsReceivedKeyValuePairDatabase
            = new KeyValuePairDatabaseMesh<long, AssociateRequestsReceived>(
                DatabaseIdentifier.UserIdToAssociateRequestsReceived.Int(),
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToAssociateRequestsReceivedDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension = ".json"
                }, new IdentifierLock<long>(), UserIdToNodeId.Instance);
        }
        public AssociateRequestsSent GetSentRequests(long userId)
        {
            return _UserIdToAssociateRequestsSentKeyValuePairDatabase.Get(userId);
        }
        public AssociateRequestsReceived GetReceivedRequests(long userId)
        {
            return _UserIdToAssociateRequestsReceivedKeyValuePairDatabase.Get(userId);
        }
        public void ModifyReceivedRequests(long userId, Func<AssociateRequestsReceived, AssociateRequestsReceived> callback)
        {
            _UserIdToAssociateRequestsReceivedKeyValuePairDatabase.ModifyWithinLock(userId, (associateRequestsReceived) => {
                if(associateRequestsReceived==null)
                    associateRequestsReceived = new AssociateRequestsReceived();
                return callback(associateRequestsReceived);
            });
        }
        public void ModifySentRequests(long userId, Func<AssociateRequestsSent, AssociateRequestsSent> callback)
        {
            _UserIdToAssociateRequestsSentKeyValuePairDatabase.ModifyWithinLock(userId, (associateRequestsSent) => {
                if (associateRequestsSent == null)
                    associateRequestsSent = new AssociateRequestsSent();
                return callback(associateRequestsSent);
            });
        }
    }
}
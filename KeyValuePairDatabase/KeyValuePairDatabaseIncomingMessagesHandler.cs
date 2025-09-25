using Core.Exceptions;
using JSON;
using Core.Handlers;
using InterserverComs;
using Core;
using KeyValuePairDatabase;

namespace KeyValuePairDatabases
{
    public class KeyValuePairDatabaseIncomingMessagesHandler
    {
        private static KeyValuePairDatabaseIncomingMessagesHandler _Instance;
        public static KeyValuePairDatabaseIncomingMessagesHandler Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(KeyValuePairDatabaseIncomingMessagesHandler));
            _Instance = new KeyValuePairDatabaseIncomingMessagesHandler();
            return _Instance;
        }
        public static KeyValuePairDatabaseIncomingMessagesHandler Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(KeyValuePairDatabaseIncomingMessagesHandler));
                return _Instance;

            }
        }
        private Action _HandleRemoveMappings;
        protected KeyValuePairDatabaseIncomingMessagesHandler()
        {
            _HandleRemoveMappings = InterserverMessageTypeMappingsHandler.Instance.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>>
                {
                    { InterserverMessageTypes.KeyValuePairDatabaseRequest, HandleMessage}
                });
        }
        private Dictionary<int, IIdentifiedKeyValuePairDatabaseMesh> _MapDatabaseIdentifierToKeyValuePairDatabaseMesh = 
            new Dictionary<int, IIdentifiedKeyValuePairDatabaseMesh>();

        public void Add(IIdentifiedKeyValuePairDatabaseMesh database)
        {
            lock (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh)
            {
                if (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh.ContainsKey(database.DatabaseIdentifier))
                    throw new DuplicateKeyException($"Database with {nameof(database.DatabaseIdentifier)} {database.DatabaseIdentifier} is already registered");
                _MapDatabaseIdentifierToKeyValuePairDatabaseMesh[database.DatabaseIdentifier] = database;
            }
        }
        public void Remove(IIdentifiedKeyValuePairDatabaseMesh database)
        {
            lock (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh)
            {
                _MapDatabaseIdentifierToKeyValuePairDatabaseMesh.Remove(database.DatabaseIdentifier);
            }
        }
        protected void HandleMessage(InterserverMessageEventArgs e)
        {
            RemoteOperationRequest remoteOperationRequest = Json.Deserialize<RemoteOperationRequest>(e.JsonString);
            IIdentifiedKeyValuePairDatabaseMesh database = null;
            int databaseIdentifier = remoteOperationRequest.DatabaseIdentifier;
            lock (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh)
            {
                if (!_MapDatabaseIdentifierToKeyValuePairDatabaseMesh.TryGetValue(databaseIdentifier, out database))
                    throw new KeyNotFoundException($"Could not find database with {nameof(remoteOperationRequest.DatabaseIdentifier)} {databaseIdentifier}");
            }
            database.HandleMessageForThisParticularDatabase(e, remoteOperationRequest);
        }
    }
}
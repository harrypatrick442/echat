using Core.Exceptions;
using JSON;
using Core.Handlers;
using InterserverComs;
using Core;
using MessageTypes.Internal;

namespace KeyValuePairDatabases.Appended
{
    public class AppendedKeyValuePairDatabaseIncomingMessagesHandler
    {
        private static AppendedKeyValuePairDatabaseIncomingMessagesHandler _Instance;
        public static AppendedKeyValuePairDatabaseIncomingMessagesHandler Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(AppendedKeyValuePairDatabaseIncomingMessagesHandler));
            _Instance = new AppendedKeyValuePairDatabaseIncomingMessagesHandler();
            return _Instance;
        }
        public static AppendedKeyValuePairDatabaseIncomingMessagesHandler Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(AppendedKeyValuePairDatabaseIncomingMessagesHandler));
                return _Instance;

            }
        }
        private Action _HandleRemoveMappings;
        protected AppendedKeyValuePairDatabaseIncomingMessagesHandler()
        {
            _HandleRemoveMappings = InterserverMessageTypeMappingsHandler.Instance.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.AppendedRead,HandleAppendedRead },
                    {InterserverMessageTypes.AppendedAppend,HandleAppendedAppend }
                });
        }
        private Dictionary<int, IAppendedKeyValuePairDatabaseIncomingMessagesHandler> _MapDatabaseIdentifierToKeyValuePairDatabaseMesh = 
            new Dictionary<int, IAppendedKeyValuePairDatabaseIncomingMessagesHandler>();

        public void Add(IAppendedKeyValuePairDatabaseIncomingMessagesHandler database)
        {
            lock (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh)
            {
                if (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh.ContainsKey(database.DatabaseIdentifier))
                    throw new DuplicateKeyException($"Database with {nameof(database.DatabaseIdentifier)} {database.DatabaseIdentifier} is already registered");
                _MapDatabaseIdentifierToKeyValuePairDatabaseMesh[database.DatabaseIdentifier] = database;
            }
        }
        public void Remove(IAppendedKeyValuePairDatabaseIncomingMessagesHandler database)
        {
            lock (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh)
            {
                _MapDatabaseIdentifierToKeyValuePairDatabaseMesh.Remove(database.DatabaseIdentifier);
            }
        }
        protected void HandleAppendedRead(InterserverMessageEventArgs e)
        {
            AppendedReadRequest request = Json.Deserialize<AppendedReadRequest>(e.JsonString);
            IAppendedKeyValuePairDatabaseIncomingMessagesHandler database = GetDatabase(request.DatabaseIdentifier);
            database.HandleAppendedRead(e, request);
        }
        protected void HandleAppendedAppend(InterserverMessageEventArgs e)
        {
            AppendedAppendRequest request = Json.Deserialize<AppendedAppendRequest>(e.JsonString);
            IAppendedKeyValuePairDatabaseIncomingMessagesHandler database = GetDatabase(request.DatabaseIdentifier);
            database.HandleAppendedAppend(e, request);
        }
        private IAppendedKeyValuePairDatabaseIncomingMessagesHandler GetDatabase(int databaseIdentifier)
        {
            IAppendedKeyValuePairDatabaseIncomingMessagesHandler database = null;
            lock (_MapDatabaseIdentifierToKeyValuePairDatabaseMesh)
            {
                if (!_MapDatabaseIdentifierToKeyValuePairDatabaseMesh.TryGetValue(databaseIdentifier,
                    out database))
                    throw new KeyNotFoundException($"Could not find database with {nameof(databaseIdentifier)} {databaseIdentifier}");
            }
            return database;
        }
    }
}
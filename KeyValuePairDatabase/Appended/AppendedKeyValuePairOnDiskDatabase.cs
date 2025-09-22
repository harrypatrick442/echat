
using JSON;
using Core.Ids;
using Core.Exceptions;
using InterserverComs;
using Nodes;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.Enums;

namespace KeyValuePairDatabases.Appended
{
    public partial class AppendedKeyValuePairOnDiskDatabase<TEntry> : IAppendedKeyValuePairDatabaseIncomingMessagesHandler //where TEntry : class
    {
        private const long MAX_FILE_LENGTH_BYTES = 2000;
        private IIdentifierLock<long> _IdentifierLock;
        private KeyValuePairDatabase<long, AppendedMetadata> _MapFileIdToAppendedLineMetadataKeyValuePairDatabase;
        private DirectoryInfo _DirectoryInfoRoot;
        private int _NCharactersEachLevel;
        private IJsonParser<TEntry> _JsonParser;
        private IIdentifierToNodeId<long> _IdentifierToNodeId;
        private bool _ThrowOnFailParseJsonObject;
        private InterserverPort _InterserverPort;
        private InterserverEndpoints _InterserverEndpoints { get { return _InterserverPort.InterserverEndpoints; } }
        private int _DatabaseIdentifier;
        public int DatabaseIdentifier { get { return _DatabaseIdentifier; } }
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();

        private INodes _Nodes { get { return Nodes.Nodes.Instance; } }
        public AppendedKeyValuePairOnDiskDatabase(string rootDirectory,
            int nCharactersEachLevel,
            IJsonParser<TEntry> jsonParser,
            bool throwOnFailParseJsonObject,
            IIdentifierToNodeId<long> identifierToNodeId,
            int databaseIdentifier, IIdentifierLock<long> identifierLock)
        {
            _DatabaseIdentifier = databaseIdentifier;
            _NCharactersEachLevel = nCharactersEachLevel;
            _JsonParser = jsonParser;
            _ThrowOnFailParseJsonObject=throwOnFailParseJsonObject;
            _IdentifierToNodeId = identifierToNodeId;
            _InterserverPort = InterserverPort.Instance;
            _DirectoryInfoRoot = new DirectoryInfo(rootDirectory.TrimEnd('\\', '/'));
            _IdentifierLock = identifierLock != null ? identifierLock : new IdentifierLock<long>();
            _MapFileIdToAppendedLineMetadataKeyValuePairDatabase =
            new KeyValuePairDatabase<long, AppendedMetadata>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams{
                    RootDirectory=rootDirectory,
                    NCharactersEachLevel=2,
                    Extension = ".json"
                    }, new NoIdentifierLock<long>());
            AppendedKeyValuePairDatabaseIncomingMessagesHandler.Instance.Add(this);
        }
        public void Append(long identifier, TEntry entry, out long indexToContinueFromToGoBackFromMessage)
        {
            string jsonString = Json.Serialize(entry);
            int nodeId = _IdentifierToNodeId.GetNodeIdFromIdentifier(identifier);
            if (_Nodes.Me.Id == nodeId)
            {
                _Append_Here(identifier, jsonString, out indexToContinueFromToGoBackFromMessage);
                return;
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            AppendedAppendResponse response =
                InterserverTicketedSender.Send<AppendedAppendRequest,
            AppendedAppendResponse>(
                    new AppendedAppendRequest(identifier, jsonString, _DatabaseIdentifier),
                    GlobalConstants.Timeouts.TIMEOUT_REMOTE_LOCKING_OPERATION, 
                    _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            indexToContinueFromToGoBackFromMessage = (long)response.IndexToContinueFromToGoBackFromMessage;
            if (!response.Successful)
                throw new OperationFailedException($"{nameof(Append)}");
        }
        public TEntry[] ReadBackwardsFromEnd(long identifier,
            int nEntries, out long indexToReadFromBackwardsExclusive)
        {
            return Read(identifier, null, nEntries, out indexToReadFromBackwardsExclusive);
        }
        public TEntry[] ContinueReadBackwards(long identifier, long? indexToReadFromBackwardsExclusive,
            int nEntries, out long toIndexFromBeginningExclusive)
        {
            if (indexToReadFromBackwardsExclusive < 1) throw new Exception($"{nameof(indexToReadFromBackwardsExclusive)} cannot be less than 1");
            return Read(identifier, indexToReadFromBackwardsExclusive, nEntries,
                out toIndexFromBeginningExclusive);
        }
        public TEntry[] Read(long identifier,
            long? indexToReadFromBackwardsExclusive,
            int nEntries, out long toIndexFromBeginningExclusive)
        {
            int nodeId = _IdentifierToNodeId.GetNodeIdFromIdentifier(identifier);
            if (_Nodes.Me.Id == nodeId)
            {
                List<string> entryStrings = _Read_Here(identifier, 
                    indexToReadFromBackwardsExclusive, nEntries, out toIndexFromBeginningExclusive);
                return _DeserializeWithExceptionHandling(entryStrings);
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            AppendedReadResponse response =
                InterserverTicketedSender.Send<AppendedReadRequest,
            AppendedReadResponse>(
                    new AppendedReadRequest(identifier, nEntries, indexToReadFromBackwardsExclusive, _DatabaseIdentifier),
                    GlobalConstants.Timeouts.TIMEOUT_REMOTE_LOCKING_OPERATION, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            if (!response.Successful)
                throw new OperationFailedException($"{nameof(Read)}");
            toIndexFromBeginningExclusive = response.ToIndexFromBeginningExclusive;
            return _DeserializeWithExceptionHandling(response.Entries);
        }
        private void _HandleIfFailedResponse(RemoteOperationResponse response)
        {
            if (response.Success) return;
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new OperationFailedException(response.ErrorMessage);
            throw new OperationFailedException("Returned success=false but no error message");

        }
        private TEntry[] _DeserializeWithExceptionHandling(IEnumerable<string>jsonStrings)
        {
            return jsonStrings.Select(_DeserializeWithExceptionHandling).Where(e => e != null).ToArray();
        }
        private TEntry _DeserializeWithExceptionHandling(string jsonString)
        {
            try
            {
                return Json.Deserialize<TEntry>(jsonString);
            }
            catch (Exception ex)
            {
                if (_ThrowOnFailParseJsonObject)
                    throw new Exception($"Failed to parse {jsonString} to {typeof(TEntry).Name}", ex);
            }
            return default(TEntry);
        }
        public void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
            AppendedKeyValuePairDatabaseIncomingMessagesHandler.Instance.Remove(this);
        }
    }
}
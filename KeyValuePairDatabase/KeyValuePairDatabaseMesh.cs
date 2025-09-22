using JSON;
using Core.Exceptions;
using Core.Ticketing;
using Core.Ids;
using Logging;
using InterserverComs;
using Nodes;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.Enums;

namespace KeyValuePairDatabases
{
    public partial class KeyValuePairDatabaseMesh<TIdentifier, TEntry> : IIdentifiedKeyValuePairDatabaseMesh, IKeyValuePairDatabase<TIdentifier, TEntry>
    {
        private const int TIMEOUT_MILLISECONDS = 5000;
        public delegate int DelegateGetNodeForIdentifier<TIdentifier>(TIdentifier identifier);
        private DelegateGetNodeForIdentifier<TIdentifier> _GetNodeIdForIdentifier;
        private IKeyValuePairDatabase<TIdentifier, TEntry> _KeyValuePairDatabase;
        private CancellationTokenSource _CancellationTokenSourceDisposed;
        private InterserverPort _InterserverPort { get { return InterserverPort.Instance; } }
        private InterserverEndpoints _InterserverEndpoints { get { return _InterserverPort.InterserverEndpoints; } }
        private INodes _Nodes;

        private int _DatabaseIdentifier;
        public int DatabaseIdentifier
        {
            get { return _DatabaseIdentifier; }
        }

        public KeyValuePairDatabaseMesh(
            int databaseIdentifier,
            OnDiskDatabaseType onDiskDatabaseType,
            OnDiskDatabaseParams onDiskDatabaseParams,
            IIdentifierLock<TIdentifier> identifierLock,
            IIdentifierToNodeId<TIdentifier> identifierToNodeId,
            bool inMemoryOnlyAllowedElseAlwaysWriteToDiskToo = false,
            IKeyValuePairDatabase<TIdentifier, TEntry> localDatabase = null)

        {
            _Nodes = Nodes.Nodes.Instance;
            _DatabaseIdentifier = databaseIdentifier;
            if(identifierToNodeId!=null)
                _GetNodeIdForIdentifier = identifierToNodeId.GetNodeIdFromIdentifier;
            _CancellationTokenSourceDisposed = new CancellationTokenSource();
            _KeyValuePairDatabase = localDatabase ??new KeyValuePairDatabase<TIdentifier, TEntry>(
                onDiskDatabaseType,
                onDiskDatabaseParams,
                 identifierLock, inMemoryOnlyAllowedElseAlwaysWriteToDiskToo);
            KeyValuePairDatabaseIncomingMessagesHandler.Instance.Add(this);

        }
        public void HandleMessageForThisParticularDatabase(
            InterserverMessageEventArgs e, RemoteOperationRequest remoteOperationRequest)
        {
            HandleKeyValuePairDatabaseRequest(e.EndpointFrom, remoteOperationRequest, e.JsonString);
            e.Handled = true;
            return;
        }
        public bool HasNotCountingNull(TIdentifier identifier)
        {
            int nodeId = _GetNodeIdForIdentifier(identifier);
            return Has(nodeId, identifier);
        }
        private bool Has(int nodeId, TIdentifier identifier)
        {
            if (_Nodes.Me.Id == nodeId)
            {
                return _KeyValuePairDatabase.HasNotCountingNull(identifier);
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            _InterserverEndpoints.GetEndpoint(nodeId);
            RemoteOperationResponse response =
                InterserverTicketedSender.Send<RemoteOperationRequest,
                RemoteOperationResponse>(
                    new RemoteOperationRequest(
                        _DatabaseIdentifier,
                            Operation.Has, identifier),
                    TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            HandleIfFailedResponse(response);
            return response.DeserializePayload<bool>();
        }
        public TEntry GetOutsideLock(TIdentifier identifier)
        {
            int nodeId = _GetNodeIdForIdentifier(identifier);
            return GetOutsideLock(nodeId, identifier);
        }
        private TEntry GetOutsideLock(int nodeId, TIdentifier identifier)
        {
            if (_Nodes.Me.Id == nodeId)
            {
                return _KeyValuePairDatabase.GetOutsideLock(identifier);
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            _InterserverEndpoints.GetEndpoint(nodeId);
            RemoteOperationResponse response =
                InterserverTicketedSender.Send<RemoteOperationRequest,
                RemoteOperationResponse>(
                    new RemoteOperationRequest(
                        _DatabaseIdentifier,
                            Operation.GetOutsideLock, identifier),
                    TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            HandleIfFailedResponse(response);
            return response.DeserializePayload<TEntry>();
        }
        public bool GetThenDeleteWithinLock(TIdentifier identifier,
            Func<TEntry, bool> callback)
        {
            int nodeId = _GetNodeIdForIdentifier(identifier);
            return GetThenDeleteWithinLock(nodeId, identifier, callback);
        }
        private bool GetThenDeleteWithinLock(int nodeId, TIdentifier identifier,
            Func<TEntry, bool> callback)
        {
            if (_Nodes.Me.Id == nodeId)
            {
                return _KeyValuePairDatabase.GetThenDeleteWithinLock(identifier, callback);
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            RemoteOperationResponse response =
                InterserverTicketedSender.Send<RemoteOperationRequest,
                RemoteOperationResponse>(
                    new RemoteOperationRequest(
                        _DatabaseIdentifier,
                            Operation.GetThenDeleteWithinLock, identifier),
                    TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            HandleIfFailedResponse(response);
            TEntry entry = response.DeserializePayload<TEntry>();
            RemoteOperationResponse responseToInverseTicketedRequest;
            try
            {
                bool result = callback(entry);
                responseToInverseTicketedRequest = 
                    InterserverTicketedSender.Send<
                        RemoteOperationResponse,
                    RemoteOperationResponse>(
                        new RemoteOperationResponse(
                                response.InverseTicket, Serialize(result), true, null),
                        TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            }
            catch (Exception ex)
            {
                //Logs.KeyValuePairDatabase.Info(ex);
                try
                {
                    SendDropInverseTicket(endpoint, response.InverseTicket);
                }
                catch
                { //Will timeout if this fails
                }
                throw new OperationFailedException("Something went wrong with inverse ticketed response request", ex);
            }
            HandleIfFailedResponse(responseToInverseTicketedRequest);
            return responseToInverseTicketedRequest.DeserializePayload<bool>();
        }
        public void ModifyWithinLock(TIdentifier identifier,
            Func<TEntry, TEntry> callback)
        {
            int nodeId = _GetNodeIdForIdentifier(identifier);
            ModifyWithinLock(nodeId, identifier, callback);
        }
        public void ModifyWithinLock(TIdentifier identifier, int nodeId,
            Func<TEntry, TEntry> callback)
        {
            ModifyWithinLock(nodeId, identifier, callback);
        }
        private void ModifyWithinLock(int nodeId, TIdentifier identifier,
            Func<TEntry, TEntry> callback)
        {
            if (_Nodes.Me.Id == nodeId)
            {
                _KeyValuePairDatabase.ModifyWithinLock(identifier, callback);
                return;
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            RemoteOperationResponse response =
                InterserverTicketedSender.Send<RemoteOperationRequest,
                RemoteOperationResponse>(
                    new RemoteOperationRequest(
                        _DatabaseIdentifier,
                            Operation.ModifyWithinLock, identifier),
                    TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            HandleIfFailedResponse(response);
            TEntry entry = response.DeserializePayload<TEntry>();//
            RemoteOperationResponse responseToInverseTicketedRequest;
            try
            {
                TEntry result = callback(entry);
                responseToInverseTicketedRequest = 
                    InterserverTicketedSender.Send<RemoteOperationResponse,
                    RemoteOperationResponse>(
                        new RemoteOperationResponse(
                                response.InverseTicket, Serialize(result), true, null),
                        TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            }
            catch (Exception ex)
            {
                //Logs.KeyValuePairDatabase.Info(ex);
                try
                {
                    SendDropInverseTicket(endpoint, response.InverseTicket);
                }
                catch
                { //Will timeout if this fails
                }
                throw new OperationFailedException("Something went wrong with inverse ticketed response request", ex);
            }
            HandleIfFailedResponse(responseToInverseTicketedRequest);
        }
        public TEntry Get(TIdentifier identifier)
        {
            int nodeId = _GetNodeIdForIdentifier(identifier);
            return Get(nodeId, identifier);
        }
        public TEntry Get(TIdentifier identifier, int nodeId)
        {
            return Get(nodeId, identifier);
        }
        private TEntry Get(int nodeId, TIdentifier identifier)
        {
            if (_Nodes.Me.Id == nodeId)
            {
                return _KeyValuePairDatabase.Get(identifier);
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            RemoteOperationResponse response =
                InterserverTicketedSender.Send<RemoteOperationRequest,
                RemoteOperationResponse>(
                    new RemoteOperationRequest(
                        _DatabaseIdentifier,    
                            Operation.Get, identifier),
                    TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            HandleIfFailedResponse(response);
            return response.DeserializePayload<TEntry>();
        }
        public void Set(TIdentifier identifier, TEntry entry, bool cacheInMemory = false)
        {

            int nodeId = _GetNodeIdForIdentifier(identifier);
            Set(nodeId, identifier, entry, cacheInMemory);
        }
        private void Set(int nodeId, TIdentifier identifier, TEntry entry, bool cacheInMemory)
        {
            if (_Nodes.Me.Id == nodeId)
            {
                _KeyValuePairDatabase.Set(identifier, entry, cacheInMemory);
                return;
            }
            if (cacheInMemory) throw new NotImplementedException(nameof(cacheInMemory));
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            RemoteOperationResponse response =
                InterserverTicketedSender.Send<RemoteOperationRequest,
                RemoteOperationResponse>(
                    new RemoteOperationRequest(
                        _DatabaseIdentifier,
                           Serialize(entry), Operation.Set, identifier),
                    TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            HandleIfFailedResponse(response);
        }
        public void Delete(TIdentifier identifier)
        {
            int nodeId = _GetNodeIdForIdentifier(identifier);
            Delete(nodeId, identifier);
        }
        private void Delete(int nodeId, TIdentifier identifier)
        {
            if (_Nodes.Me.Id == nodeId)
            {
                _KeyValuePairDatabase.Delete(identifier);
                return;
            }
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
            RemoteOperationResponse response =
                InterserverTicketedSender.Send<RemoteOperationRequest,
                RemoteOperationResponse>(
                    new RemoteOperationRequest(
                        _DatabaseIdentifier, Operation.Delete, identifier),
                    TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
            HandleIfFailedResponse(response);
        }
        private void HandleIfFailedResponse(RemoteOperationResponse response)
        {
            if (response.Success) return;
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new OperationFailedException(response.ErrorMessage);
            throw new OperationFailedException("Returned success=false but no error message");

        }
        private void SendDropInverseTicket(INodeEndpoint endpoint, long inverseTicket)
        {
            endpoint.SendJSONString(Json.Serialize(new DropInverseTicketMessage(inverseTicket)));
        }
        private static string Serialize<TPayload>(TPayload payload) {
            return Json.Serialize(payload);
        }
        //TODO must support multiple named instances and must implement dispose. 
        public void Dispose()
        {
            _CancellationTokenSourceDisposed.Cancel();
            _KeyValuePairDatabase.Dispose();
            KeyValuePairDatabaseIncomingMessagesHandler.Instance.Remove(this);
        }
    }
}
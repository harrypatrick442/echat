using JSON;
using Logging;
using InterserverComs;
using Core.Messages.Messages;

namespace KeyValuePairDatabases
{
    public partial class KeyValuePairDatabaseMesh<TIdentifier, TEntry>
    {
        private void HandleKeyValuePairDatabaseRequest(INodeEndpoint endpointFrom,
            RemoteOperationRequest remoteOperationRequest, string jsonString)
        {
            //I recognise the inefficiency with multiple deserializations but ill come back to this
            //For now this is reliable and will be good for sometime.
            
            switch (remoteOperationRequest.Operation)
            {
                case Operation.Get:
                    HandleGet(endpointFrom, remoteOperationRequest);
                    break;
                case Operation.Set:
                    HandleSet(endpointFrom, jsonString);
                    break;
                case Operation.Delete:
                    HandleDelete(endpointFrom, remoteOperationRequest);
                    break;
                case Operation.Has:
                    HandleHas(endpointFrom, remoteOperationRequest);
                    break;
                case Operation.GetOutsideLock:
                    HandleGetOutsideLock(endpointFrom, remoteOperationRequest);
                    break;
                case Operation.GetThenDeleteWithinLock:
                    HandleGetThenDeleteWithinLock(endpointFrom, remoteOperationRequest);
                    break;
                case Operation.ModifyWithinLock:
                    HandleModifyWithinLock(endpointFrom, remoteOperationRequest);
                    break;
            }
        }
        private void HandleGetOutsideLock(INodeEndpoint endpointFrom, RemoteOperationRequest remoteOperationRequest)
        {
            try
            {
                TEntry entry = _KeyValuePairDatabase.GetOutsideLock((TIdentifier)remoteOperationRequest.Identifier);
                Send(remoteOperationRequest.Ticket, endpointFrom, new RemoteOperationResponse(Serialize(entry), true, null));
            }
            catch (Exception ex)
            {
                Send( remoteOperationRequest.Ticket, endpointFrom, new RemoteOperationResponse(ex));
            }
        }
        private void HandleGetThenDeleteWithinLock(INodeEndpoint endpointFrom, RemoteOperationRequest remoteOperationRequest)
        {
            RemoteOperationResponse inverseTicketedResponse = null;
            try
            {
                bool deleted = _KeyValuePairDatabase.GetThenDeleteWithinLock((TIdentifier)remoteOperationRequest.Identifier, (entry) => {
  
                        inverseTicketedResponse = InterserverInverseTicketedSender
                        .Send<RemoteOperationResponse, RemoteOperationResponse>(new RemoteOperationResponse(Serialize(entry), true, null),
                        remoteOperationRequest.Ticket, TIMEOUT_MILLISECONDS, _CancellationTokenSourceDisposed.Token,
                        endpointFrom.SendJSONString);
                        HandleIfFailedResponse(inverseTicketedResponse);
                        return inverseTicketedResponse.DeserializePayload<bool>();
                });
                Send(inverseTicketedResponse.Ticket, endpointFrom, 
                    new RemoteOperationResponse(Serialize(deleted), true, null));
            }
            catch (Exception ex)
            {
                Send(inverseTicketedResponse!=null? inverseTicketedResponse.Ticket:remoteOperationRequest.Ticket, endpointFrom, new RemoteOperationResponse(ex));
            }

        }
        private void HandleModifyWithinLock(INodeEndpoint endpointFrom, RemoteOperationRequest remoteOperationRequest)
        {
            RemoteOperationResponse inverseTicketedResponse = null;
            try
            {
                _KeyValuePairDatabase.ModifyWithinLock((TIdentifier)remoteOperationRequest.Identifier, (entry) => {
                    inverseTicketedResponse = InterserverInverseTicketedSender
                        .Send<RemoteOperationResponse, RemoteOperationResponse>(
                            new RemoteOperationResponse(Serialize(entry), true, null),
                        remoteOperationRequest.Ticket,
                            TIMEOUT_MILLISECONDS,
                            _CancellationTokenSourceDisposed.Token,
                            endpointFrom.SendJSONString
                        );
                    //tHIS DOESNT seem to be getting the inverse ticketed response even though its sent almost immediately
                    ///but when send in the catch is called suddenly the response it should have already got comes through.
                    HandleIfFailedResponse(inverseTicketedResponse); 
                    return inverseTicketedResponse.DeserializePayload<TEntry>();
                });
                Send(inverseTicketedResponse.Ticket, endpointFrom, RemoteOperationResponse.Successful());
            }
            catch (Exception ex)
            {
                Send(inverseTicketedResponse != null ? inverseTicketedResponse.Ticket : remoteOperationRequest.Ticket, endpointFrom, new RemoteOperationResponse(ex));
            }
        }
        private void HandleSet(INodeEndpoint endpointFrom, string jsonString)
        {
            RemoteOperationRequest remoteOperationRequest = Json.Deserialize <RemoteOperationRequest>(jsonString);
            RemoteOperationResponse remoteOperationResponse;
            try
            {
                _KeyValuePairDatabase.Set((TIdentifier)remoteOperationRequest.Identifier, remoteOperationRequest.DeserializePayload<TEntry>());
                remoteOperationResponse = new RemoteOperationResponse(true, null);
            }
            catch (Exception ex)
            {
                remoteOperationResponse = new RemoteOperationResponse(ex);
            }
            Send(remoteOperationRequest.Ticket, endpointFrom, remoteOperationResponse);
        }
        private void HandleDelete(INodeEndpoint endpointFrom, RemoteOperationRequest remoteOperationRequest)
        {
            RemoteOperationResponse remoteOperationResponse;
            try
            {
                _KeyValuePairDatabase.Delete((TIdentifier)remoteOperationRequest.Identifier);
                remoteOperationResponse = new RemoteOperationResponse(true, null);
            }
            catch (Exception ex)
            {
                remoteOperationResponse = new RemoteOperationResponse(ex);
            }
            Send(remoteOperationRequest.Ticket, endpointFrom, remoteOperationResponse);
        }
        private void HandleGet(INodeEndpoint endpointFrom, RemoteOperationRequest remoteOperationRequest)
        {
            RemoteOperationResponse remoteOperationResponse;
            try
            {
                TEntry entry = _KeyValuePairDatabase.Get((TIdentifier)remoteOperationRequest.Identifier);
                remoteOperationResponse = new RemoteOperationResponse(Serialize(entry), true, null);
            }
            catch (Exception ex)
            {
                remoteOperationResponse = new RemoteOperationResponse(ex);
            }
            Send(remoteOperationRequest.Ticket, endpointFrom, remoteOperationResponse);
        }
        private void HandleHas(INodeEndpoint endpointFrom,
            RemoteOperationRequest remoteOperationRequest)
        {
            RemoteOperationResponse remoteOperationResponse;
            try
            {
                bool has= _KeyValuePairDatabase.HasNotCountingNull((TIdentifier)remoteOperationRequest.Identifier);
                remoteOperationResponse = new RemoteOperationResponse(Serialize(has), true, null);
            }
            catch (Exception ex)
            {
                remoteOperationResponse = new RemoteOperationResponse(ex);
            }
            Send(remoteOperationRequest.Ticket, endpointFrom, remoteOperationResponse);
        }
        private void Send<TMessage>(long ticket, INodeEndpoint endpointFrom, TMessage message) where TMessage:TicketedMessageBase{
            message.Ticket = ticket;
            endpointFrom.SendJSONString(Json.Serialize(message));
        }
    }
}
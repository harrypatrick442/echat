using Core.Messages.Messages;
using Nodes;
using Core.Exceptions;
using JSON;
using Logging;
using DependencyManagement;
using ConfigurationCore;

namespace InterserverComs
{
    public static class OperationRedirectHelper
    {
        public static void OperationRedirectedToNode<TRemoteRequest, TRemoteResponse>(
            int nodeId, Action callbackDoHere,
            Func<TRemoteRequest> callbackCreateRequest,
            Action<TRemoteResponse> didRemotely,
            CancellationToken cancellationToken)
            where TRemoteRequest : ITicketedMessageBase where TRemoteResponse : ITicketedMessageBase
        {
            if (nodeId == Nodes.Nodes.Instance.MyId)
            {
                callbackDoHere();
                return;
            }
            INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(nodeId);
            if (nodeEndpoint == null)
                throw new OperationFailedException($"Failed to get {nameof(INodeEndpoint)} for node with id {nodeId}");
            TRemoteResponse removeAssociateResponse = InterserverTicketedSender.Send<TRemoteRequest, TRemoteResponse>(
                callbackCreateRequest(),
                DependencyManager.Get<ITimeoutsConfiguration>().TimeoutRemoteOperation,
                cancellationToken, nodeEndpoint.SendJSONString
            );
            didRemotely(removeAssociateResponse);
        }
        public static void OperationRedirectedToNode<TRemoteRequest>(
            int nodeId, Action callbackDoHere,
            Func<TRemoteRequest> callbackCreateRequest)
            where TRemoteRequest : TypedMessageBase
        {
            if (nodeId == Nodes.Nodes.Instance.MyId)
            {
                callbackDoHere();
                return;
            }
            INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(nodeId);
            if (nodeEndpoint == null)
                throw new OperationFailedException($"Failed to get {nameof(INodeEndpoint)} for node with id {nodeId}");
            nodeEndpoint.SendJSONString(Json.Serialize(callbackCreateRequest()));
        }
        public static void SendMessageObjectToNodes<TMessage>(TMessage message,
            IEnumerable<int> nodeIds, bool throwExceptions= false) where TMessage : TypedMessageBase
        {
            string serialized = Json.Serialize(message);
            List<Exception> exceptions = null;
            foreach (int nodeId in nodeIds) {
                try
                {
                    if (nodeId == Nodes.Nodes.Instance.MyId)
                    {
                        InterserverMessageTypeMappingsHandler.Instance.HandleMessage(new InterserverMessageEventArgs(null, message.Type, serialized));
                        continue;
                    }
                    INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(nodeId);
                    if (nodeEndpoint == null)
                        throw new OperationFailedException($"Failed to get {nameof(INodeEndpoint)} for node with id {nodeId}");
                    nodeEndpoint.SendJSONString(serialized);
                }
                catch(Exception ex){
                    if (!throwExceptions) {
                        Logs.Default.Error(ex);
                        continue;
                    }
                    if (exceptions == null)
                        exceptions = new List<Exception> { ex };
                    else exceptions.Add(ex);
                }
            }
            if (throwExceptions && exceptions.Count > 0)
                throw new AggregateException(exceptions.ToArray());
        }
    }
}
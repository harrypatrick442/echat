using Core.Exceptions;
using Nodes;
using Logging;
using InterserverComs.Delegates;

namespace InterserverComs
{
    public class InterserverWebsocketClient : AuthenticatedNodeWebsocketClientBase
    {
        public InterserverWebsocketClient(long thisMachineNodeId,
            InterserverConnection interserverConnection,
            DelegateHandleMessage handleMessage, string publicKeyPath) 
            : base(thisMachineNodeId, GlobalConstants.Endpoints.INTERSERVER_WEBSOCKET,
                interserverConnection.ServerUrl, handleMessage,
                interserverConnection.Password, publicKeyPath, 
                interserverConnection.NodeId)
        {
            base.BeginConnecting();
        }
        public override void SendJSONString(string jsonString)
        {
            try
            {
                base.SendJSONString(jsonString);
            }
            catch (Exception ex)
            {
                throw new OperationFailedException($"Error sending to node {NodeId}", ex);
            }
        }
    }
}

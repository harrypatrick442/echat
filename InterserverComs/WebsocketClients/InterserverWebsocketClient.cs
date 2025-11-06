using Core.Exceptions;
using Nodes;
using Logging;
using InterserverComs.Delegates;
using DependencyManagement;
using ConfigurationCore;

namespace InterserverComs
{
    public class InterserverWebsocketClient : AuthenticatedNodeWebsocketClientBase
    {
        public InterserverWebsocketClient(long thisMachineNodeId,
            InterserverConnection interserverConnection,
            DelegateHandleMessage handleMessage, string publicKeyPath) 
            : base(thisMachineNodeId, DependencyManager.Get<IEndpointsConfiguration>().InterserverWebsocket,
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

using Core.Exceptions;
using JSON;
using Shutdown;
using WebSocketSharp.Server;
using Nodes;
using MessageTypes.Internal;
using Core.Messages.Messages;
using Logging;
using Core;

namespace InterserverComs
{
    public sealed class InterserverPort : IShutdownable
    {
        private static InterserverPort _Instance;
        public static InterserverPort Initialize(WebSocketServer webSocketServer, string publicKeyPath) {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(InterserverPort));
            _Instance = new InterserverPort(webSocketServer, publicKeyPath);
            return _Instance;
        }
        public static InterserverPort Instance { get
            {

                if (_Instance == null) throw new NotInitializedException(nameof(InterserverPort));
                return _Instance;
            } }
        private string _PublicKeyPath;
        public ShutdownOrder ShutdownOrder => ShutdownOrder.InterserverPort;

        private InterserverEndpoints _InterserverEndpoints;
        public InterserverEndpoints InterserverEndpoints
        {
            get
            {
                return _InterserverEndpoints;
            }
        }
        private InterserverPort(
            WebSocketServer webSocketServer, string publicKeyPath)
        {
            _PublicKeyPath = publicKeyPath;
            InterserverMessageTypeMappingsHandler.Initialize();
            webSocketServer.AddWebSocketService<InterserverWebsocketServer>(
                GlobalConstants.Endpoints.INTERSERVER_WEBSOCKET, InitializeWebSocketServerInstance);
            _InterserverEndpoints = new InterserverEndpoints(Nodes.Nodes.Instance.Me,
                _HandleMessage, publicKeyPath);
            ShutdownManager.Instance.Add(this);
        }
        public NodeEndpointState[] GetNodeEndpointStates() {
            return InterserverEndpoints.AsArray().Select(endpoint=>endpoint.NodeEndpointState.Clone()).ToArray();
        }
        public INodeEndpoint GetEndpointByNodeId(int nodeId)
        {
            return _InterserverEndpoints.GetEndpoint(nodeId);
        }
        private void _HandleMessage(INodeEndpoint endpointFrom, string jsonString) {
            TypedInverseTicketedMessage typedTicketedMessage = Json.Deserialize<TypedInverseTicketedMessage>(jsonString);
            //TODO can inject this into InterserverMessageTypeMappingsHandler too
            if (InterserverTicketedSender.HandleMessage(typedTicketedMessage, jsonString)) return;
            if (InterserverInverseTicketedSender.HandleMessage(typedTicketedMessage, jsonString)) return;
            if (typedTicketedMessage.Type == InterserverMessageTypes.TestInterserverConnection)
            {
                endpointFrom.SendJSONString(Json.Serialize(new TestInterserverConnectionResponseMessage(typedTicketedMessage.Ticket)));
                return;
            }
            InterserverMessageEventArgs interserverMessageEventArgs = new InterserverMessageEventArgs(endpointFrom, typedTicketedMessage.Type, jsonString);
            if (InterserverMessageTypeMappingsHandler.Instance.HandleMessage(interserverMessageEventArgs))
                return;
        }
        private void InitializeWebSocketServerInstance(InterserverWebsocketServer instance)
        {
            instance.EmitOnPing = true;
            instance.IgnoreExtensions = true;
            instance.CookiesValidator = (req, res) =>
            {
                return true; // If valid.
            };
            instance.Initialize(_InterserverEndpoints.AddIncomingEndpoint, _HandleMessage);
        }
        public InterserverConnectionState[] GetConnectionsStates(bool includeExceptions = true)
        {
            List<InterserverConnectionState> interserverConnectionStates = new List<InterserverConnectionState>();
            InterserverConnection[] interserverConnections = Nodes.Nodes.Instance.Me.InterserverConnections;
            CountdownLatch countdownLatch = new CountdownLatch(interserverConnections.Count());
            foreach (InterserverConnection interserverConnection in interserverConnections)
            {
                new Thread(() =>
                {
                    try
                    {
                        INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(interserverConnection.NodeId);
                        if (endpoint == null)
                        {
                            lock (interserverConnectionStates)
                            {
                                interserverConnectionStates.Add(
                                    new InterserverConnectionState(endpoint.NodeId,
                                    isOpen:false,
                                    gotResponsePingSuccessfully: false, includeExceptions? new Exception("No endpoint currently mapped"):null)
                                );
                            }
                            return;
                        }
                        bool gotResponsePingSuccessfully = false;
                        Exception exception = null;
                        try
                        {
                            TestInterserverConnectionResponseMessage response = InterserverTicketedSender
                                .Send<TestInterserverConnectionMessage, TestInterserverConnectionResponseMessage>(
                                    new TestInterserverConnectionMessage(),
                                    2000, null, endpoint.SendJSONString);
                            gotResponsePingSuccessfully = true;
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        lock (interserverConnectionStates)
                        {
                            interserverConnectionStates.Add(
                                new InterserverConnectionState(endpoint.NodeId, endpoint.IsOpen, gotResponsePingSuccessfully,
                                    includeExceptions ? exception : null)
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.Default.Error(ex);
                    }
                    finally
                    {
                        countdownLatch.Signal();
                    }
                }).Start();
            }
            countdownLatch.Wait();
            return interserverConnectionStates.ToArray();
        }
        public void Dispose()
        {
            _InterserverEndpoints.Dispose();
        }
    }
}
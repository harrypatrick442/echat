using Logging;
using Nodes;
using Core;
using Core.Events;
using Core.Exceptions;
using System.Linq;
using InterserverComs.Delegates;

namespace InterserverComs
{
    public class InterserverEndpoints : IDisposable
    {
        INode _NodeMe;
        public event EventHandler<NodeEndpointEventArgs> InterserverConnectionOpened;
        public event EventHandler<NodeEndpointEventArgs> InterserverConnectionClosed;
        private bool _Disposed = false;
        private Dictionary<int, INodeEndpoint> _MapNodeIdToEndpoint = new Dictionary<int, INodeEndpoint>();
        private string _PublicKeyPath;
        private DelegateHandleMessage _HandleMessage;
        public InterserverEndpoints(INode nodeMe, DelegateHandleMessage handleMessage, string publicKeyPath)
        {
            if (nodeMe == null)
                throw new NullReferenceException($"{nameof(nodeMe)} was null");
            _HandleMessage = handleMessage;
            _PublicKeyPath = publicKeyPath; 
            _NodeMe = nodeMe;
            new Thread(CreateMyOutgoingClientConnections).Start();
        }
        private void CreateMyOutgoingClientConnections()
        {
            InterserverConnection[] interserverConnections = _NodeMe.InterserverConnections;
            InterserverConnection[] clientInterserverConnections = interserverConnections
                .Where(i => i.IAmClientElseServer).ToArray();
            if (clientInterserverConnections.Length < 1) return;
            CountdownLatch countdownLatch = new CountdownLatch(clientInterserverConnections.Count());
            List<Exception> exceptions = new List<Exception>();
            foreach (InterserverConnection interserverConnection in clientInterserverConnections)
            {
                new Thread(() =>
                {
                    try
                    {
                        CreateOutgoingClientConnection(interserverConnection);
                        Logs.Default.Info($"Managed to connect to {interserverConnection.NodeId} on startup");
                    }
                    catch (Exception ex)
                    {
                        Logs.Default.Info($"Failed to connect to {interserverConnection.NodeId} on startup");
                        Logs.Default.Error(ex);
                        if (ex != null)
                        {
                            lock (exceptions)
                            {
                                exceptions.Add(ex);
                            }
                        }
                    }
                    finally {
                        countdownLatch.Signal();
                    }
                }).Start();
            }
            countdownLatch.Wait();
            //Does not throw an exception here upon failure to connect a WebSocket.
            if (exceptions.Any()) { 
                throw new AggregateException(exceptions.ToArray());
            }
        }
        public INodeEndpoint[] AsArray()
        {
            lock (_MapNodeIdToEndpoint)
            {
                return _MapNodeIdToEndpoint.Values.ToArray();
            }
        }
        public INodeEndpoint GetEndpoint(int nodeId)
        {
            lock (_MapNodeIdToEndpoint)
            {
                if (_MapNodeIdToEndpoint.TryGetValue(nodeId, out INodeEndpoint endpoint))
                    return endpoint;
                return null;
            }
        }
        /// <summary>
        /// For us being server side
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="endpoint"></param>
        /// <param name="replaceExisting"></param>
        public void AddIncomingEndpoint(INodeEndpoint endpoint)
        {
            int nodeId = endpoint.NodeId;
            if (nodeId == _NodeMe.Id) throw new FatalException($"Something went very wrong. The incoming {nameof(nodeId)} was mine");
            InterserverConnection interserverConnectionToNode = _NodeMe.GetInterserverConnectionTo(nodeId);
            if (interserverConnectionToNode == null)
                throw new InvalidOperationException("This should never happen because how did we authenticate this node without the interserverConnection object");
            lock (_MapNodeIdToEndpoint)
            {
                if (interserverConnectionToNode.IAmClientElseServer)
                {
                    RemoveEventHandlersToNewNodeEndpoint(endpoint);
                    endpoint.Dispose();
                    return;
                }
                if (_MapNodeIdToEndpoint.TryGetValue(nodeId, out INodeEndpoint existing))
                {
                    RemoveEventHandlersToNewNodeEndpoint(existing);
                    existing.Dispose();
                }
                _MapNodeIdToEndpoint[nodeId] = endpoint;
                AddEventHandlersToNewNodeEndpoint(endpoint);
            }
        }
        private void CreateOutgoingClientConnection(InterserverConnection interserverConnection)
        {
            int toNodeId = interserverConnection.NodeId;
            lock (_MapNodeIdToEndpoint)
            {
                if (_Disposed)
                    return;
                if (_MapNodeIdToEndpoint.TryGetValue(toNodeId, out INodeEndpoint existingEndpoint))
                {
                    existingEndpoint.Dispose();
                    //TODO can remove new one cos of how old one gets disposed.existing, after new one mapped
                    RemoveEventHandlersToNewNodeEndpoint(existingEndpoint);
                    _MapNodeIdToEndpoint.Remove(toNodeId);
                }
            }
            INodeEndpoint newEndpoint = new InterserverWebsocketClient(
                Nodes.Nodes.Instance.MyId, interserverConnection,
                _HandleMessage, _PublicKeyPath);
            lock (_MapNodeIdToEndpoint)
            {
                _MapNodeIdToEndpoint[toNodeId] = newEndpoint;
                AddEventHandlersToNewNodeEndpoint(newEndpoint);
            }
        }
        private void AddEventHandlersToNewNodeEndpoint(INodeEndpoint newEndpoint)
        {

            newEndpoint.OnOpened += HandleInterserverConnectionOpened;
            newEndpoint.OnClosed += HandleInterserverConnectionClosed;
        }
        private void RemoveEventHandlersToNewNodeEndpoint(INodeEndpoint newEndpoint)
        {

            newEndpoint.OnOpened -= HandleInterserverConnectionOpened;
            newEndpoint.OnClosed -= HandleInterserverConnectionClosed;
        }
        private void HandleInterserverConnectionOpened(object sender, NodeEndpointEventArgs e)
        {
            InterserverConnectionOpened?.Invoke(this, e);
        }
        private void HandleInterserverConnectionClosed(object sender, NodeEndpointEventArgs e)
        {

            InterserverConnectionClosed?.Invoke(this, e);
        }
        public void Dispose()
        {
            lock (_MapNodeIdToEndpoint)
            {
                if (_Disposed) return;
                _Disposed = true;
            }
        }
    }
}
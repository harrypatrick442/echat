
using Core.Exceptions;
using Core.Timing;
using Logging;
using WebSocketSharp;
using Nodes;

namespace InterserverComs
{
    public class InterserverWebsocketServer :
        AuthenticatedNodeWebsocketServerBase, INodeEndpoint
    {
        private readonly object _LockObjectDispose = new object();
        private bool _Disposed = false;
        public int NodeId => _NodeThisEndpointGoesTo.Id;
        private readonly object _LockObjectConnectedTimestamp = new object();
        private long _ConnectedTimestamp;
        public long ConnectedTimestamp
        {
            get
            {
                lock (_LockObjectConnectedTimestamp)
                {
                    return _ConnectedTimestamp;
                }
            }
        }

        public long InstanceId { get; protected set; }
        private NodeEndpointState _NodeEndpointState;
        public NodeEndpointState NodeEndpointState
        {
            get
            {
                _NodeEndpointState.IsOpen = IsOpen;
                return _NodeEndpointState;
            } 
        }

        private InterserverConnection _InterserverConnection;
        protected Action<INodeEndpoint, string> _CallbackHandleMessage;
        protected Action<INodeEndpoint> _CallbackAddIncomingEndpoint;
        private List<EventHandler<NodeEndpointEventArgs>> _OnOpenedEventHandlers = new List<EventHandler<NodeEndpointEventArgs>>();
        private bool _OpenForEvents = false;
        public event EventHandler<NodeEndpointEventArgs> OnOpened
        {
            add {
                bool open;
                lock (_OnOpenedEventHandlers) {
                    _OnOpenedEventHandlers.Add(value);
                    open = _OpenForEvents;
                }
                if (open) value.Invoke(this, new NodeEndpointEventArgs(this));
            }
            remove
            {
                lock (_OnOpenedEventHandlers)
                {
                    _OnOpenedEventHandlers.Remove(value);
                }
            }
        }
        public event EventHandler<NodeEndpointEventArgs> OnClosed;
        public InterserverWebsocketServer():base(false)
        {
            InstanceId = -1;
        }
        public virtual void Initialize(Action<INodeEndpoint> callbackAddIncomingEndpoint, Action<INodeEndpoint, string> callbackHandleMessage)
        {
            INodes nodes = Nodes.Nodes.Instance;
            base.Initialize(nodes);
            _Nodes = nodes;
            _CallbackAddIncomingEndpoint = callbackAddIncomingEndpoint;
            _CallbackHandleMessage = callbackHandleMessage;
        }
        protected override void OnOpen()
        {
            try
            {
                base.OnOpen();
                string instanceId = Context?.QueryString[NodeEndpointStateDataMemberNames.InstanceId];
                if (!string.IsNullOrEmpty(instanceId))
                {
                    InstanceId = long.Parse(instanceId);
                }
                _NodeEndpointState = new NodeEndpointState(NodeId, InstanceId, ConnectedTimestamp, iAmClient: false);
                NodeEndpointStatesHistory.Add(_NodeEndpointState);
                NodeEndpointStatesHistory.Opened(InstanceId);
                NodeEndpointStatesHistory.OpenEvent(InstanceId);
                lock (_LockObjectConnectedTimestamp)
                {
                    _ConnectedTimestamp = TimeHelper.MillisecondsNow;
                }
                //If made it this far no exception then is authenticated
                _InterserverConnection = _Nodes.Me.GetInterserverConnectionTo(_NodeThisEndpointGoesTo.Id);
                InterserverConnectionChanged(_InterserverConnection);
                _CallbackAddIncomingEndpoint(this);
                DispatchOnOpened();
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                throw ex;
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            NodeEndpointStatesHistory.Closed(InstanceId);
            NodeEndpointStatesHistory.CloseEvent(InstanceId);
        }
        private void DispatchOnOpened() {
            new Thread(()=>{
                EventHandler<NodeEndpointEventArgs>[] onOpenedEventHandlers;
                lock (_OnOpenedEventHandlers)
                {
                    _OpenForEvents = true;
                    onOpenedEventHandlers = _OnOpenedEventHandlers.ToArray();
                }
                NodeEndpointEventArgs nodeEndpointEventArgs = new NodeEndpointEventArgs(this);
                foreach (EventHandler<NodeEndpointEventArgs> onOpenEventHandler in onOpenedEventHandlers)
                {
                    onOpenEventHandler.Invoke(this, nodeEndpointEventArgs);
                }
            }).Start();
        }
        private void DispatchOnClose() {

            new Thread(() => { 
                OnClosed?.Invoke(this, new NodeEndpointEventArgs(this));
            }).Start();
        }
        private void InterserverConnectionChanged(InterserverConnection interserverConnection)
        {
            if (interserverConnection.IAmClientElseServer)
            {
                Dispose();
            }
        }
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            base.OnError(e);
            Logs.Default.Error(e.Exception);
        }
        protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
        {
            base.OnMessage(e);
            new Thread(() =>
            {
                try
                {
                    Logs.Default.Info($"Receive {e.Data}");
                    _CallbackHandleMessage(this, e.Data);

                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }

        public override void Dispose()
        {
            base.Dispose();
            lock (_LockObjectDispose)
            {
                if (_Disposed) return;
                _Disposed = true;
            }
        }

        public override void SendJSONString(string jsonString)
        {
            try
            {
                Logs.Default.Info($"Send {jsonString}");
                base.SendJSONString(jsonString);
            }
            catch (Exception ex) {
                throw new OperationFailedException($"Error sending to node {NodeId}", ex);
            }
        }
    }
}
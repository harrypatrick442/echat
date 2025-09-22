
using WebSocketSharp;
using Core.Timing;
using Logging;
using Nodes;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using InterserverComs.Delegates;

namespace InterserverComs
{
    public abstract class AuthenticatedNodeWebsocketClientBase : INodeEndpoint
    {
        private readonly object _LockObjectDisposed = new object();
        private bool _Disposed = false;

        public int NodeId { get; }
        public long SessionId { get { return -1; } }
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

        DelegateHandleMessage _HandleMessage;
        private WebSocket _WebSocket;
        private WebsocketReconnectTimer _ReconnectTimer;

        private List<EventHandler<NodeEndpointEventArgs>> _OnOpenedEventHandlers = new List<EventHandler<NodeEndpointEventArgs>>();
        private bool _OpenForEvents = false;
        public event EventHandler<NodeEndpointEventArgs> OnOpened
        {
            add
            {
                bool open;
                lock (_OnOpenedEventHandlers)
                {
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
        public bool IsOpen{ get { return _WebSocket.ReadyState.Equals(WebSocketState.Open); } }

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

        protected AuthenticatedNodeWebsocketClientBase(long thisMachineNodeId,
                string websocketEndpoint, string serverUrl,
                DelegateHandleMessage handleMessage,
                string password, string publicKeyPath, int nodeId) : base()
        {
            NodeId = nodeId;
            _HandleMessage = handleMessage;
            string url = serverUrl; //"ws://localhost:8080";
            if (url.Last() != '/')
                url += '/';
            if (websocketEndpoint[0] == '/')
                websocketEndpoint = websocketEndpoint.Substring(1);
            url = $"{url}{websocketEndpoint}?{AuthenticatedNodeWebsocketServerBase.NODE_QUERY_STRING_KEY}={thisMachineNodeId}&{AuthenticatedNodeWebsocketServerBase.PASSWORD_QUERY_STRING_KEY}={password}&{NodeEndpointStateDataMemberNames.InstanceId}={InstanceId}";
            bool usingTLS = url.ToLower().IndexOf("wss") == 0;
            if (usingTLS&& publicKeyPath == null)
            {
                throw new ArgumentException(nameof(publicKeyPath));
            }
            _NodeEndpointState = new NodeEndpointState(nodeId, InstanceId, ConnectedTimestamp, iAmClient: true);
            NodeEndpointStatesHistory.Add(_NodeEndpointState);
            InstanceId = TimeHelper.MillisecondsNow;//TODO NOT SAFE?
            _WebSocket = new WebSocket(url);
            if (usingTLS)
            {
                _WebSocket.SslConfiguration.CheckCertificateRevocation = false;
                _WebSocket.SslConfiguration.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
                _WebSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
                X509Certificate2 cert = new X509Certificate2(publicKeyPath, "", X509KeyStorageFlags.MachineKeySet);
                _WebSocket.SslConfiguration.ClientCertificateSelectionCallback =
                    (sender, targethost, localCertificates, remoteCertificate, acceptableIssuers) =>
                    {
                        return cert;
                    };
            }
            _ReconnectTimer = new WebsocketReconnectTimer(Reconnect);
            _WebSocket.OnMessage += HandleMessage;
            _WebSocket.OnClose += HandleClose;
            _WebSocket.OnError += HandleError;
            _WebSocket.OnOpen += HandleOpen;
        }
        public void BeginConnecting()
        {
            _WebSocket.Connect();
        }
        ~AuthenticatedNodeWebsocketClientBase()
        {
            Dispose();
        }
        private void Reconnect()
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
            }
            _WebSocket.Connect();//TODO OperationNotSupported exception can be thrown here.
        }

        public void Dispose()
        {
            lock (_LockObjectDisposed) {
                if (_Disposed) return;
                _Disposed = true;
            }
            _ReconnectTimer.Dispose();
            _WebSocket.OnMessage -= HandleMessage;
            _WebSocket.OnClose -= HandleClose;
            _WebSocket.OnError -= HandleError;
            _WebSocket.OnOpen -= HandleOpen;
            _WebSocket.Close();
            new Thread(DispatchOnClosed).Start();
        }

        public virtual void SendJSONString(string jsonString)
        {
            Logs.Default.Info($"Send {jsonString}");
            _WebSocket.Send(jsonString);
        }
        protected virtual void HandleError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Logs.Default.Error(e.Exception);
        }

        protected virtual void HandleClose(object sender, CloseEventArgs e)
        {
            NodeEndpointStatesHistory.Closed(InstanceId);
            NodeEndpointStatesHistory.CloseEvent(InstanceId);
            //We have to be very careful with an underlying library because events could cause lockup conditions. So run on new thread.
            new Thread(() =>
            {
                DispatchOnClosed();
                _ReconnectTimer.Closed();
            }).Start();
        }
        protected virtual void HandleOpen(object sender, EventArgs e)
        {
            _ReconnectTimer.Opened();
            NodeEndpointStatesHistory.Opened(InstanceId);
            NodeEndpointStatesHistory.OpenEvent(InstanceId);
            lock (_LockObjectConnectedTimestamp)
            {
                _ConnectedTimestamp = TimeHelper.MillisecondsNow;
            }
            //We have to be very careful with an underlying library because events could cause lockup conditions. So run on new thread.
            DispatchOnOpened();
        }
        protected void HandleMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            //We have to be very careful with an underlying library because events could cause lockup conditions. So run on new thread.
            new Thread(() =>
            {
                try
                {
                    Logs.Default.Info($"Receive {e.Data}");
                    _HandleMessage(this, e.Data);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }
        private void DispatchOnOpened()
        {
            new Thread(() =>
            {
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
        private void DispatchOnClosed()
        {
            EventHandler<NodeEndpointEventArgs> onClosed = OnClosed;
            if (onClosed == null) return;
            try
            {
                onClosed.Invoke(this, new NodeEndpointEventArgs(this));
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
            }
        }
    }
}

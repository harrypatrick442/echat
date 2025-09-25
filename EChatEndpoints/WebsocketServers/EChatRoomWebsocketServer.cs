using JSON;
using WebSocketSharp;
using WebSocketSharp.Server;
using Logging;
using System.Net;
using Chat;
using Core.InterserverComs;
using Core.Interfaces;
using Chat.Endpoints;
using WebAbstract;

namespace EChatEndpoints.WebsocketServers
{
    public class EChatRoomWebsocketServer : WebSocketBehavior, IClientEndpointLight
    {
        protected static readonly Json _JsonParser = new Json();
        private IPAddress? _IPAddress = null;
        private readonly object _LockObject = new object();
        private bool _Disposed;
        private ChatRoomClientEndpoint _ChatRoomClientEndpoint;
        private ChatRoomAuthenticationClientEndpoint _ChatRoomAuthenticateClientEndpoint;
        private ClientMessageTypeMappingsHandler _ClientMessageTypeMappingsHandler;
        private IATClientEndpoint _IATClientEndpoint;
        private long _ConversationId;
        public EChatRoomWebsocketServer()
        {
            _ClientMessageTypeMappingsHandler = new ClientMessageTypeMappingsHandler();
            _IATClientEndpoint = new IATClientEndpoint(
                this, IATCallback, _ClientMessageTypeMappingsHandler);
        }
        private void IATCallback(bool authenticated, long userId) {
            lock (_LockObject)
            {
                if (_Disposed) return;
                if (authenticated)
                {
                    if (_ChatRoomAuthenticateClientEndpoint != null)
                        _ChatRoomAuthenticateClientEndpoint.Dispose();
                    _ChatRoomAuthenticateClientEndpoint = new ChatRoomAuthenticationClientEndpoint(
                        this, _ConversationId, userId, _ClientMessageTypeMappingsHandler, _IPAddress,
                        EnterRoom, Dispose);
                }
            }
        }
        private void EnterRoom(ChatRoom chatRoom, long userId)
        {
            ChatRoomClientEndpoint chatRoomClientEndpoints;
            lock (_LockObject)
            {
                if (_Disposed) return;
                if (_ChatRoomClientEndpoint != null) return;
                _ChatRoomClientEndpoint = (chatRoomClientEndpoints=chatRoom.Add(userId,
                    _ClientMessageTypeMappingsHandler, this));
            }
            chatRoomClientEndpoints.OnDispose += _HandleClientEndpointDisposed;
            if (chatRoomClientEndpoints.Disposed)
            {
                Dispose();
            }
        }
        private void _HandleClientEndpointDisposed(object? sender, EventArgs e)
        {
            Dispose();
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Dispose();
        }
        protected override void OnOpen()
        {
            base.OnOpen();
            try
            {
                string? conversationIdString = Context?.QueryString[GlobalConstants.Parameters.CONVERSATION_ID];
                if (conversationIdString == null) throw new ArgumentNullException(nameof(conversationIdString));
                lock (_LockObject)
                {
                    _ConversationId = long.Parse(conversationIdString);
                    _IPAddress = Context?.UserEndPoint?.Address;
                }
            }
            catch (Exception ex)
            {
                Dispose();
                Logs.Default.Error(ex);
            }
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            _ClientMessageTypeMappingsHandler.HandleMessageOnNewThread(e.Data);
        }
        public void SendObject<TObject>(TObject obj)
        {
            SendJSONString(Json.Serialize(obj));
        }
        public void SendJSONString(string jsonString)
        {
            try
            {
                Send(jsonString);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        public virtual void Dispose()
        {
            try
            {
                Context?.WebSocket?.Close();
            }
            catch { }
            lock (_LockObject)
            {
                if (_Disposed) return;
                _Disposed = true;
                if (_IATClientEndpoint != null)
                {
                    _IATClientEndpoint.Dispose();
                }
                if (_ChatRoomAuthenticateClientEndpoint != null)
                {
                    _ChatRoomAuthenticateClientEndpoint.Dispose();
                }
                if (_ChatRoomClientEndpoint != null)
                {
                    _ChatRoomClientEndpoint.OnDispose -= _HandleClientEndpointDisposed;
                    _ChatRoomClientEndpoint.Dispose();
                }
            }
        }
    }
}

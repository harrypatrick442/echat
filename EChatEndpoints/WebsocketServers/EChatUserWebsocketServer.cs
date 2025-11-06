using JSON;
using WebSocketSharp;
using WebSocketSharp.Server;
using Logging;
using System.Net;
using Core.InterserverComs;
using Core.Interfaces;
using Users;
using UserIgnore;
using Authentication;
using Core.Exceptions;
using Users.DAL;    
using UserRouting;  
using Sessions;
using Chat.Endpoints;
using EChatEmailing;
using Flagging.Endpoints;
using NotificationsCore;
using NotificationsCore.Messages.Messages;
using EChat.Messages;
using Users.Messages.Client;
using WebAbstract;
using UserMultimediaCore;
using MentionsCore;
using HashTags;
using WebAbstract.ClientEndpoints;

namespace EChatEndpoints.WebsocketServers
{
    //So there are two types of websocket on e-caht. A single one for the user which handles things like querying profile information for other users logging in
    //and lots of other things potentially. Pms also use this websocket. Pms use the original distributed system i designed for that.
    //Then we have the chatroom type of websocket. That opens for an individual chatroom to whatever server is hosting it.
    //This saves a lot of interserver coms and massively improves performance. And from a human perspective it makes sense cos how many damn 
    //chatrooms can someone really be in at once and keeping up.
    //How many websockets can a browser easily handle. Many more than chatrooms a person can handle at once.
    //So notifications related to a chat room will actually go through this websocket but we not at that yet.
    //basic stuff working first.
    public class EChatUserWebsocketServer : WebSocketBehavior, IClientEndpoint, IUserChatClientEndpoint
    {
        protected static readonly Json _JsonParser = new Json();
        private static HashSet<EChatUserWebsocketServer> _Instances = new HashSet<EChatUserWebsocketServer>();
        public static int NInstances {
            get{ 
                lock(_Instances)
                    return _Instances.Count; 
            }
        }
        private IPAddress _ClientIPAddress;
        private ClientMessageTypeMappingsHandler _ClientMessageTypeMappingsHandler;
        private AuthenticatedClientEndpoint _AuthenticatedClientEndpoint;
        private AssociatesClientEndpoint _AssociatesClientEndpoint;
        private UserMultimediaClientEndpoint _MultimediaClientEndpoint;
        private IgnoreClientEndpoint _IgnoreClientEndpoint;
        public ChatClientEndpoint ChatClientEndpoint { get; }
        private MentionsClientEndpoint _MentionsClientEndpoint;
        private HashTagsClientEndpoint _HashTagsClientEndpoint;
        private FlaggingClientEndpoint _FlaggingClientEndpoint;
        private UserNotificationsClientEndpoint _UserNotificationsClientEndpoint;
        public long SessionId => _AuthenticatedClientEndpoint.SessionInfoSafe.SessionId;
        public ISessionInfo SessionInfoSafe => _AuthenticatedClientEndpoint.SessionInfoSafe;
        public long UserId {
            get
            {
                SessionInfo sessionInfo = _AuthenticatedClientEndpoint.SessionInfoSafe;
                if (sessionInfo != null)
                    return sessionInfo.UserId;
                return -1;
            }
        }

        public IPAddress ClientIPAddress => _ClientIPAddress;

        public bool HasSession => _AuthenticatedClientEndpoint.SessionInfoSafe != null;

        private readonly object _LockObjectDisposed = new object();
        private bool _Disposed = false;
        private string _UserAgent;
        public EChatUserWebsocketServer() : base()
        {
            _ClientMessageTypeMappingsHandler = new ClientMessageTypeMappingsHandler();
            _AuthenticatedClientEndpoint = new AuthenticatedClientEndpoint(
                CreateNewUser, this,
                CoreUserRoutingTable.Instance, 
                _ClientMessageTypeMappingsHandler,
                getUserAgent:()=>_UserAgent,
                GetAdditionalPayloadForAuthenticatedUser,
                debugLoggingEnabled:false
            );;
            _FlaggingClientEndpoint = new FlaggingClientEndpoint(
                _ClientMessageTypeMappingsHandler, this);
            _AssociatesClientEndpoint = new AssociatesClientEndpoint(
                _ClientMessageTypeMappingsHandler, this);
            _IgnoreClientEndpoint = new IgnoreClientEndpoint(
                _ClientMessageTypeMappingsHandler, this);
            ChatClientEndpoint = new ChatClientEndpoint(_ClientMessageTypeMappingsHandler, this);
            _MultimediaClientEndpoint = new UserMultimediaClientEndpoint(this, _ClientMessageTypeMappingsHandler);
            _MentionsClientEndpoint = new MentionsClientEndpoint(_ClientMessageTypeMappingsHandler, this);
            _HashTagsClientEndpoint= new HashTagsClientEndpoint(_ClientMessageTypeMappingsHandler, this);
            _UserNotificationsClientEndpoint = new UserNotificationsClientEndpoint(_ClientMessageTypeMappingsHandler, this);
            lock (_Instances)
            {
                _Instances.Add(this);
            }
        }

        private long CreateNewUser(bool guest, string username)
        {
            long userId = UserIdSource.Instance.NextId();
            DalUserProfiles dalUserProfiles = Users.DAL.DalUserProfiles.Instance;
            dalUserProfiles.ModifyUserProfile(userId, (existingUserProfile) =>
            {
                if (existingUserProfile != null) throw new FatalException("This should never happen");
                return new UserProfile(userId, guest, username);
            });
            dalUserProfiles.UsernameSearchAddUser(userId, username);
            return userId;
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Dispose/*Endpoint*/();
        }
        protected override void OnOpen()
        {
            base.OnOpen();
            try
            {
                _UserAgent = Context?.Headers["User-Agent"];
                _ClientIPAddress = Context?.UserEndPoint?.Address;
            }
            catch { return; }
            try
            {
                string? token = Context?.QueryString[Configurations.Parameters.TOKEN];
                if (token != null)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            _AuthenticatedClientEndpoint.DoOnOpen(_ClientIPAddress, token, null);
                        }
                        catch (Exception ex)
                        {
                            Logs.Default.Error(ex);
                        }
                    }).Start();
                }
            }
            catch { }
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            _ClientMessageTypeMappingsHandler.HandleMessageOnNewThread(e.Data);
        }

        public void SendObject<TObject>(TObject obj) where TObject : class
        {
            SendJSONString(Json.Serialize(obj));
        }
        public virtual void SendJSONString(string jsonString)
        {
            try
            {
                base.Send(jsonString);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void Close()
        {
            try
            {
                Context?.WebSocket?.Close();
            }
            catch { }
        }
        private void _HandleClientEndpointDisposed(object? sender, EventArgs e)
        {
            Dispose();
        }
        ~EChatUserWebsocketServer()
        {
            Dispose();
        }
        public virtual void Dispose()
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                _Disposed = true;
            }
            lock (_Instances)
            {
                _Instances.Remove(this);
            }
            Close();
            DisposeEndpoint();
        }
        private void DisposeEndpoint()
        {
            _FlaggingClientEndpoint.Dispose();
            _AuthenticatedClientEndpoint.Dispose();
            _AssociatesClientEndpoint.Dispose(); 
            ChatClientEndpoint.Dispose();
            _MultimediaClientEndpoint.Dispose();
            _IgnoreClientEndpoint.Dispose();
            _MentionsClientEndpoint.Dispose();
            _HashTagsClientEndpoint.Dispose();
            _UserNotificationsClientEndpoint.Dispose();
        }
        private object GetAdditionalPayloadForAuthenticatedUser(long userId) {
            UserNotifications? userNotifications = UserNotificationsMesh.Instance.Get(userId);
            UsersMesh.Instance.GetAllAssociateEntries(userId,
                out UserProfileSummary[] associates,
                out AssociateRequestUserProfileSummarys receivedRequests,
                out AssociateRequestUserProfileSummarys sentRequests);
            return new AdditionalPayloadsOnSignIn(
                userNotifications,
                new GetAllAssociateEntriesResponse(associates, receivedRequests, sentRequests, 0));
        }
    }
}

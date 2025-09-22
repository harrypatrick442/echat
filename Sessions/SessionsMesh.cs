using Core.Exceptions;
using Shutdown;
using InterserverComs;
using Sessions.Requests;
using Sessions.Responses;
using Logging;
using Nodes;

namespace Sessions
{
    public partial class SessionsMesh
    {
        private static SessionsMesh? _Instance;
        public static SessionsMesh Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(SessionsMesh));
            _Instance = new SessionsMesh();
            return _Instance;
        }
        public static SessionsMesh Instance { 
            get { 
                if (_Instance == null)
                    throw new NotInitializedException(nameof(SessionsMesh));
                return _Instance;
            } 
        }
        private int _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private SessionsMesh() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.SessionsMesh);
        }
        public bool Authenticate(string iatString, out long userId)
        {
            try
            {
                InterserverAuthenticationToken iat = InterserverAuthenticationToken.Parse(iatString);
                return Authenticate(iat.NodeId, iat.SessionId, iat.Token, out userId);
            }
            catch {
                userId = 0;
                return false;
            }
        }
        public bool Authenticate(int nodeId, long sessionId, string token, out long userId)
        {
            try
            {
                bool authenticated = false;
                long userIdInternal = 0;
                OperationRedirectHelper.OperationRedirectedToNode<
                    SessionsGetTokenRequest, SessionsGetTokenResponse>(nodeId,
                    () => {
                        
                        long? userIdHere = Authenticate_Here(sessionId, token);
                        if (userIdHere == null) return;
                        userIdInternal = (long)userIdHere;
                        authenticated = true;

                    },
                    () => new SessionsGetTokenRequest(sessionId, token),
                    (response) =>
                    {
                        if (response.UserId == null) return;
                        userIdInternal = (long)response.UserId;
                        authenticated = true;
                    },
                    _CancellationTokenSourceDisposed.Token
               );
                userId = userIdInternal;
                return authenticated;
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                userId = 0;
                return false;
            }
        }
        private void Dispose()
        {
            _CancellationTokenSourceDisposed.Cancel();
        }
    }
}
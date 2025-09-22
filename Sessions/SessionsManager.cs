using Core.Exceptions;
using Core.Events;
using Timer = System.Timers.Timer;
namespace Sessions
{
    public class SessionsManager:IDisposable
    {
        private const int TIMEOUT_SECONDS = 3600*24;
        private static SessionsManager _Instance;
        public event EventHandler<ItemEventArgs<SessionInfo>> OnSessionDisposed;
        private static readonly object _LockObjectInstance = new object();
        private static readonly object _LockObjectDispose = new object();
        private bool _Disposed = false;
        private Dictionary<string, SessionInfo> _MapTokenToSessionInformation = new Dictionary<string, SessionInfo>();
        private Timer _TimerCleanupSessions;
        public static SessionsManager Instance
        {
            get
            {
                lock (_LockObjectInstance)
                {
                    if (_Instance == null) _Instance = new SessionsManager();
                    return _Instance;
                }
            }
        }
        protected SessionsManager() {
            /*
            _TimerCleanupSessions = new Timer(10000, -1, CleanupSessions);
            _TimerCleanupSessions.Start();*/
        }
        /*
        public SessionInfo CreateNewSession(long userId, string token, PermissionEnum[] permissions)
        {
            lock (_MapTokenToSessionInformation) {
                SessionInfo sessionInfo = new SessionInfo(token, userId, permissions, _HandleCallbackDispose);
                _MapTokenToSessionInformation[token]= sessionInfo;
                return sessionInfo;
            }
        }*/
        /*
        public SessionInfo CheckToken(string token, params PermissionEnum[] permissions) {
            SessionInfo sessionInfo;
            lock (_MapTokenToSessionInformation)
            {
                if (!_MapTokenToSessionInformation.ContainsKey(token)) ThrowAuthenticationFailed();
                sessionInfo = _MapTokenToSessionInformation[token];
            }
            sessionInfo.CheckPermissions(permissions);
            sessionInfo.UpdateLastAccessed();
            return sessionInfo;
        }
        /*
        private void CleanupSessions() {
            DateTime _DateTimeCutoffForTimeout = DateTime.Now.AddSeconds(-TIMEOUT_SECONDS);
            lock (_MapTokenToSessionInformation) {
                foreach (string token in _MapTokenToSessionInformation.Keys.ToArray()) {
                    SessionInfo sessionInfo = _MapTokenToSessionInformation[token];
                    if (sessionInfo.LastAccessed<_DateTimeCutoffForTimeout) {
                        _MapTokenToSessionInformation.Remove(token);
                        DispatchSessionDisposed(sessionInfo);
                    }
                }
            }
        }*/
        private void _HandleCallbackDispose(SessionInfo sessionInfo) {
                _MapTokenToSessionInformation.Remove(sessionInfo.Token);
                DispatchSessionDisposed(sessionInfo);
        }
        private void DispatchSessionDisposed(SessionInfo sessionInfo) {
            OnSessionDisposed?.Invoke(this, new ItemEventArgs<SessionInfo>(sessionInfo));
        }
        private static void ThrowAuthenticationFailed() {
            throw new BadCredentialsException("Token was not recognised");
        }
        public void Dispose()
        {
            lock (_LockObjectDispose)
            {
                if (_Disposed) return;
                _Disposed = true;
            }
            _Instance = null;
            _TimerCleanupSessions.Stop();
            _Instance = null;
        }
    }
}
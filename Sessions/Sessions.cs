using Core.Interfaces;

namespace Sessions
{
    public static class Sessions
    {
        private static Dictionary<long, SessionInfo> _MapSessionIdToSessionInfo 
            = new Dictionary<long, SessionInfo>();
        public static SessionInfo? GetById(long sessionId) { 
            lock(_MapSessionIdToSessionInfo)
            {
                _MapSessionIdToSessionInfo.TryGetValue(sessionId, out SessionInfo? sessionInfo);
                return sessionInfo;
            }
        }
        internal static void Add(SessionInfo sessionInfo)
        {
            lock (_MapSessionIdToSessionInfo)
            {
                _MapSessionIdToSessionInfo.Add(sessionInfo.SessionId, sessionInfo);
            }
        }
        internal static void Remove(long sessionId)
        {
            lock (_MapSessionIdToSessionInfo)
            {
                _MapSessionIdToSessionInfo.Remove(sessionId);
            }
        }
        public static SessionInfo New(long userId, string token,
            string deviceIdentifier, ISessionIdSource sessionIdSource)
        { 
            SessionInfo sessionInfo = SessionInfo.New(userId, token, deviceIdentifier, sessionIdSource, Remove);
            Add(sessionInfo);
            return sessionInfo;
        }
    }
}
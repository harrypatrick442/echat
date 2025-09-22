using Core.Interfaces;

namespace Sessions
{
    public class SessionInfo : ISessionInfo
    {
        public string Token { get; }
        public long UserId { get; }
        public long SessionId { get; }
        public string DeviceIdentifier { get; }
        private Action<long> _Remove;
        public SessionInfo(long userId, string token, long sessionId, string deviceIdentifier, Action<long> remove)
        {
            Token = token;
            UserId = userId;
            SessionId = sessionId;
            DeviceIdentifier = deviceIdentifier;
            _Remove = remove;
        }
        internal static SessionInfo New(long userId, string token,
            string deviceIdentifier, ISessionIdSource sessionIdSource, Action<long> remove)
        {
            return new SessionInfo(userId, token, sessionIdSource.NextId(), deviceIdentifier, remove);
        }
        public void Dispose() {
            _Remove(SessionId);
        }
    }
}
namespace Sessions
{
    public partial class SessionsMesh
    {
        public long? Authenticate_Here(long sessionId, string token)
        {
            SessionInfo? sessionInfo = Sessions.GetById(sessionId);
            if (sessionInfo != null
                && !string.IsNullOrEmpty(sessionInfo.Token)
                && sessionInfo.Token == token)
                return sessionInfo.UserId;
            return null;
        }
    }
}
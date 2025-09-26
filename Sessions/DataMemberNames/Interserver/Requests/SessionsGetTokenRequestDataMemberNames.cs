using MessageTypes.Attributes;

namespace Sessions.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.SessionsAuthenticate)]
    public static class SessionsGetTokenRequestDataMemberNames
    {
        public const string SessionId = "s";
        public const string Token = "t";
    }
}
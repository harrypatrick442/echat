namespace Sessions
{
    public sealed class InterserverAuthenticationToken
    {
        public int NodeId { get; }
        public long SessionId { get; }
        public string Token { get; }
        public InterserverAuthenticationToken(int nodeId, long sessionId, string token)
        {
            NodeId = nodeId;
            SessionId = sessionId;
            Token = token;
        }
        public override string ToString()
        {
            return $"{NodeId}_{SessionId}_{Token}";
        }
        public static InterserverAuthenticationToken Parse(string iat) {
            string[] splits = iat.Split("_");
            return new InterserverAuthenticationToken(int.Parse(splits[0]), long.Parse(splits[1]), splits[2]);
        }
    }
}
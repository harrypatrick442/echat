namespace UserRouting
{
    public interface IUserRoutingTable<TEndpoint>
    {
        void Add(long userId, long sessionId, TEndpoint endpoint);
        void Remove(long userId, long sessionId);
    }
}
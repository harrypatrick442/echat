namespace Sessions
{
    public interface ISessionIdSource
    {
        public long NextId();
    }
}
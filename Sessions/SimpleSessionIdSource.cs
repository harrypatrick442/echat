
namespace Sessions
{
    public class SimpleSessionIdSource : ISessionIdSource
    {
        private long _CurrentId = 1;
        public long NextId()
        {
            lock (this) {
                return _CurrentId++;
            }
        }
    }
}
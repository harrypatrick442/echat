

namespace KeyValuePairDatabases
{
    public class NoIdentifierLock<TIdentifier> : IIdentifierLock<TIdentifier>
    {

        public TReadResult LockForReads<TReadResult>(TIdentifier identifier, Func<TReadResult> callback)
        {

            return callback();
        }
        public void LockForReads(TIdentifier identifier, Action callback)
        {

            callback();
        }
        public void LockForWrite(TIdentifier identifier, Action callback)
        {
            callback();
        }
    }
}
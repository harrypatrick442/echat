

namespace KeyValuePairDatabases
{
    public interface IIdentifierLock<TIdentifier>
    {
        TReadResult LockForReads<TReadResult>(TIdentifier identifier, Func<TReadResult> callback);
        void LockForReads(TIdentifier identifier, Action callback);
        void LockForWrite(TIdentifier identifier, Action callback);
    }
}


namespace KeyValuePairDatabases
{
    public interface IKeyValuePairDatabase<TIdentifier, TEntry>
    {
        int DatabaseIdentifier { get; }

        void Delete(TIdentifier identifier);
        void Dispose();
        TEntry Get(TIdentifier identifier);
        TEntry GetOutsideLock(TIdentifier identifier);
        bool GetThenDeleteWithinLock(TIdentifier identifier, Func<TEntry, bool> callback);
        bool HasNotCountingNull(TIdentifier identifier);
        void ModifyWithinLock(TIdentifier identifier, Func<TEntry, TEntry> callback);
        void Set(TIdentifier identifier, TEntry entry, bool cacheInMemory = false);
    }
}
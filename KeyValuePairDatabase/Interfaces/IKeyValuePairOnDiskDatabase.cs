using Core.Delegates;

namespace KeyValuePairDatabases.Interfaces
{
    public interface IKeyValuePairOnDiskDatabase<TIdentifier, TEntry>
    {
        void Delete(TIdentifier identifier);
        TEntry Read(TIdentifier identifier);        
        void ReadCallbackDeleteWithinLock(TIdentifier identifier, Action<TEntry> callback);
        void ReadCallbackWriteWithinLock(TIdentifier identifier, Func<TEntry, TEntry> callback);
        TEntry ReadWithoutLock(TIdentifier identifier);
        void Write(TIdentifier identifier, TEntry entry);
        bool Has(TIdentifier identifier);
        void IterateEntries(Action<DelegateNextEntry<TEntry>> callback);
    }
}
using Core.Delegates;
using Core.Timing;
using KeyValuePairDatabases.Enums;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.OnDiskDatabases;

namespace KeyValuePairDatabases
{
    public class KeyValuePairDatabase<TIdentifier, TEntry>:IKeyValuePairDatabase<TIdentifier, TEntry>
    {
        public IKeyValuePairOnDiskDatabase<TIdentifier, TEntry> _KeyValuePairOnDiskDatabase;
        public KeyValuePairInMemoryDatabase<TIdentifier, TEntry> _KeyValuePairInMemoryDatabase;
        private IIdentifierLock<TIdentifier> _IdentifierLock;
        private bool _InMemoryOnlyAllowedElseAlwaysWriteToDiskToo;

        public int DatabaseIdentifier => throw new NotImplementedException();

        public KeyValuePairDatabase(
            OnDiskDatabaseType onDiskDatabaseType,
            OnDiskDatabaseParams onDiskDatabaseParams,
            IIdentifierLock<TIdentifier> identifierLock,
            bool inMemoryOnlyAllowedElseAlwaysWriteToDiskToo = false)
        {
            _IdentifierLock = identifierLock;
            _InMemoryOnlyAllowedElseAlwaysWriteToDiskToo = inMemoryOnlyAllowedElseAlwaysWriteToDiskToo;
            _KeyValuePairOnDiskDatabase = OnDiskDatabaseFactory.Create<TIdentifier, TEntry>(onDiskDatabaseType, onDiskDatabaseParams, new NoIdentifierLock<TIdentifier>());
            _KeyValuePairInMemoryDatabase = new KeyValuePairInMemoryDatabase<TIdentifier, TEntry>(
                new OverflowParameters<TIdentifier, TEntry>(overflowToNowhere:!inMemoryOnlyAllowedElseAlwaysWriteToDiskToo, 
                identifierLock, _HandleOverflowFromInMemoryDatabase),
                new NoIdentifierLock<TIdentifier>());

        }
        public bool HasNotCountingNull(TIdentifier identifier)
        {

            if (_KeyValuePairInMemoryDatabase.Get(identifier) != null)
                return true;
            return _KeyValuePairOnDiskDatabase.Read(identifier) != null;
        }
        public bool HasCountingNull(TIdentifier identifier)
        {
            if (_KeyValuePairInMemoryDatabase.Contains(identifier)) return true;
            return _KeyValuePairOnDiskDatabase.Has(identifier);
        }
        public TEntry GetOutsideLock(TIdentifier identifier) {
            TEntry entry = _KeyValuePairInMemoryDatabase.Get(identifier);
            if (entry != null) return entry;
            entry = _KeyValuePairOnDiskDatabase.Read(identifier);
            _KeyValuePairInMemoryDatabase.Set(identifier, entry);
            return entry;
        }
        public bool GetThenDeleteWithinLock(TIdentifier identifier,
            Func<TEntry, bool> callback)
        {

            bool delete = false;
            _IdentifierLock.LockForWrite(identifier, () =>
            {
                TEntry entry = _KeyValuePairInMemoryDatabase.Get(identifier);
                bool hadInMemory = entry != null;
                if (!hadInMemory)
                {
                    entry = _KeyValuePairOnDiskDatabase.Read(identifier);
                }
                delete = callback(entry);
                if (delete)
                {
                    _KeyValuePairInMemoryDatabase.Delete(identifier);
                    _KeyValuePairOnDiskDatabase.Delete(identifier);
                }
            });
            return delete;
        }
        public void ModifyWithinLock(TIdentifier identifier,
            Func<TEntry, TEntry> callback)
        {
            _IdentifierLock.LockForWrite(identifier, () =>
            {
                TEntry entry = _KeyValuePairInMemoryDatabase.Get(identifier);
                bool hadInMemory = entry != null;
                if (!hadInMemory)
                {
                    entry = _KeyValuePairOnDiskDatabase.Read(identifier);
                }
                entry = callback(entry);
                if (entry == null)
                {
                    if (hadInMemory)
                        _KeyValuePairInMemoryDatabase.Delete(identifier);
                    _KeyValuePairOnDiskDatabase.Delete(identifier);
                    return;
                }
                if (hadInMemory)
                {
                    _KeyValuePairInMemoryDatabase.Set(identifier, entry);
                    if (_InMemoryOnlyAllowedElseAlwaysWriteToDiskToo) return;
                }

                TEntry entryFromInMemoryNow = _KeyValuePairInMemoryDatabase.Get(identifier);
                //TODO Might want to promote to in memory cache here. Maybe a variable for optional
                _KeyValuePairOnDiskDatabase.Write(identifier, entry);
            });
        }

        public void ModifyWithinLockWithSet(TIdentifier identifier,
            Action<TEntry, Action<TEntry>> callback)
        {
            _IdentifierLock.LockForWrite(identifier, () =>
            {
                TEntry entry = _KeyValuePairInMemoryDatabase.Get(identifier);
                bool hadInMemory = entry != null;
                if (!hadInMemory)
                {
                    entry = _KeyValuePairOnDiskDatabase.Read(identifier);
                }
                Action<TEntry> set = (intermediate) =>
                {
                    if (intermediate == null)
                    {
                        if (hadInMemory)
                            _KeyValuePairInMemoryDatabase.Delete(identifier);
                        _KeyValuePairOnDiskDatabase.Delete(identifier);
                        return;
                    }
                    if (hadInMemory)
                    {
                        _KeyValuePairInMemoryDatabase.Set(identifier, intermediate);
                        if (_InMemoryOnlyAllowedElseAlwaysWriteToDiskToo) return;
                    }
                    //TEntry entryFromInMemoryNow = _KeyValuePairInMemoryDatabase.Get(identifier);
                    //TODO Might want to promote to in memory cache here. Maybe a variable for optional
                    _KeyValuePairOnDiskDatabase.Write(identifier, intermediate);
                };
                callback(entry, set);
            });
        }
        public TEntry Get(TIdentifier identifier)
        {
            return _IdentifierLock.LockForReads(identifier, () =>
            {
                TEntry entry = _KeyValuePairInMemoryDatabase.Get(identifier);
                if (entry != null)  
                    return entry;

                entry =_KeyValuePairOnDiskDatabase.Read(identifier);
                _KeyValuePairInMemoryDatabase.Set(identifier, entry);
                return entry;
            });
        }

        public TEntry GetIfInMemory(TIdentifier identifier)
        {
            return _IdentifierLock.LockForReads(identifier,
                () =>_KeyValuePairInMemoryDatabase.Get(identifier));
        }
        public void Set(TIdentifier identifier, TEntry entry, bool cacheInMemory = false)
        {
            _IdentifierLock.LockForWrite(identifier, () =>
            {
                if (cacheInMemory)
                {
                    _KeyValuePairInMemoryDatabase.Set(identifier, entry);
                    if (_InMemoryOnlyAllowedElseAlwaysWriteToDiskToo) return;
                }
                else
                {
                    if (_KeyValuePairInMemoryDatabase.SetIfContains(identifier, entry))
                    {
                        if (_InMemoryOnlyAllowedElseAlwaysWriteToDiskToo) return;
                    }
                }
                _KeyValuePairOnDiskDatabase.Write(identifier, entry);
            });
        }
        public void Delete(TIdentifier identifier)
        {
            _IdentifierLock.LockForWrite(identifier, () =>
            {
                _KeyValuePairInMemoryDatabase.Delete(identifier);
                _KeyValuePairOnDiskDatabase.Delete(identifier);
            });
        }
        public void IterateEntries(Action<DelegateNextEntry<TEntry>> callback) {

            _KeyValuePairOnDiskDatabase.IterateEntries(callback);
        }
        private void _HandleOverflowFromInMemoryDatabase(TIdentifier identifier, TEntry entry) {
            _KeyValuePairOnDiskDatabase.Write(identifier, entry);
        }
        public void Dispose() {
            //Although entries can technically keep coming in as it overflows. Other macro systems should already have stopped this.
            _KeyValuePairInMemoryDatabase.OverflowAllToExternal();
        }
    }
}
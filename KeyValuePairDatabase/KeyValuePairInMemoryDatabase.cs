
using Core.MemoryManagement;

namespace KeyValuePairDatabases
{
    public class KeyValuePairInMemoryDatabase<TIdentifier, TEntry>: IMemoryManaged
    {
        public event EventHandler<NEntriesChangedEventArgs> OnNEntriesChanged;
        private volatile int _NEntriesLastTimeDispatched = 0;

        private OverflowParameters<TIdentifier, TEntry> _OverflowParameters;
        private LinkedList<Tuple<TIdentifier, TEntry>> _LinkedList = new LinkedList<Tuple<TIdentifier, TEntry>>();
        private IIdentifierLock<TIdentifier> _IdentifierLock;
        private Dictionary<TIdentifier, EntryWrapper> _MapIdentifierToEntry = new Dictionary<TIdentifier, EntryWrapper>();
        private class EntryWrapper
        {
            private LinkedListNode<Tuple<TIdentifier, TEntry>> _LinkedListNode;
            public LinkedListNode<Tuple<TIdentifier, TEntry>> LinkedListNode {
                get { return _LinkedListNode; } set { _LinkedListNode = value; } }
            private TIdentifier _Identifier;
            public TIdentifier identifier { get { return _Identifier; } }
            public void UpdateEntry(TEntry entry) {
                _LinkedListNode.Value = new Tuple<TIdentifier, TEntry>(_Identifier, entry);
            }
            public EntryWrapper(LinkedListNode<Tuple<TIdentifier, TEntry>> LinkedListNode, 
                TIdentifier identifier) {
                _LinkedListNode = LinkedListNode;
                _Identifier = identifier;
            }
        }
        public KeyValuePairInMemoryDatabase(
            OverflowParameters<TIdentifier, TEntry> overflowParameters,
            IIdentifierLock<TIdentifier> identifierLock) {
            _OverflowParameters = overflowParameters;
            _IdentifierLock = identifierLock;
            MemoryManager.Instance.Add(this);
        }
        public bool Contains(TIdentifier identifier)
        {
            lock (_MapIdentifierToEntry)
            {
                return _MapIdentifierToEntry.ContainsKey(identifier);
            }
        }
        public void ReadCallbackWriteWithinLock(TIdentifier identifier,
            Func<TEntry, TEntry> callback)
        {

            _IdentifierLock.LockForWrite(identifier, () =>
            {
                TEntry entry = _Get(identifier);
                entry = callback(entry);
                if (entry != null)
                    _Set(identifier, entry);
            });
        }
        public void ReadCallbackDeleteWithinLock(TIdentifier identifier,
            Action<TEntry> callback)
        {

            _IdentifierLock.LockForWrite(identifier, () =>
            {
                TEntry entry = _Get(identifier);
                callback(entry);
                _Delete(identifier);
            });
        }
        public TReturn ReadWithinLock<TReturn>(TIdentifier identifier,
            Func<TEntry, TReturn> callback)
        {

            return _IdentifierLock.LockForReads(identifier, () =>
            {
                TEntry entry = _Get(identifier);
                return callback(entry);
            });
        }
        public void Set(TIdentifier identifier, TEntry entry)
        {
            _IdentifierLock.LockForWrite(identifier, () => {
                _Set(identifier, entry);
            });
        }
        public bool SetIfContains(TIdentifier identifier, TEntry entry)
        {
            bool contained = false;
            _IdentifierLock.LockForWrite(identifier, () => {
                lock (_MapIdentifierToEntry)
                {
                    if (_MapIdentifierToEntry.ContainsKey(identifier))
                    {
                        EntryWrapper currentEntryWrapper = _MapIdentifierToEntry[identifier];
                        _LinkedList.Remove(currentEntryWrapper.LinkedListNode);
                        currentEntryWrapper.LinkedListNode =
                            _LinkedList.AddLast(new Tuple<TIdentifier, TEntry>(identifier, entry));
                        contained = true;
                    }
                }
            });
            return contained;
        }
        public TEntry Get(TIdentifier identifier)
        {
            return _IdentifierLock.LockForReads(identifier, () =>
            {
                return _Get(identifier);
            });
        }
        public void Delete(TIdentifier identifier)
        {
            _IdentifierLock.LockForWrite(identifier, () => {
                _Delete(identifier);
            });
        }
        private void _Set(TIdentifier identifier, TEntry entry) {
            lock (_MapIdentifierToEntry) {
                if (_MapIdentifierToEntry.ContainsKey(identifier))
                {
                    EntryWrapper currentEntryWrapper = _MapIdentifierToEntry[identifier];
                    _LinkedList.Remove(currentEntryWrapper.LinkedListNode);
                    currentEntryWrapper.LinkedListNode =
                        _LinkedList.AddLast(new Tuple<TIdentifier, TEntry>(identifier, entry));
                    return;
                }
                LinkedListNode<Tuple<TIdentifier, TEntry>> LinkedListNode =
                    _LinkedList.AddLast(new Tuple<TIdentifier, TEntry>(identifier, entry));
                EntryWrapper entryWrapper = new EntryWrapper(LinkedListNode, 
                    identifier);
                _MapIdentifierToEntry[identifier] = entryWrapper;
            }
        }
        private TEntry _Get(TIdentifier identifier)
        {
            lock (_MapIdentifierToEntry)
            {
                if (_MapIdentifierToEntry.ContainsKey(identifier))
                    return _MapIdentifierToEntry[identifier].LinkedListNode.Value.Item2;
                return default(TEntry);
            }
        }
        private void _Delete(TIdentifier identifier)
        {
            lock (_MapIdentifierToEntry)
            {
                if (!_MapIdentifierToEntry.ContainsKey(identifier))
                    return;
                EntryWrapper entryWrapper = _MapIdentifierToEntry[identifier];
                _LinkedList.Remove(entryWrapper.LinkedListNode);
                _MapIdentifierToEntry.Remove(identifier);
            }
        }
        public void ReduceMemoryFootprintByProportion(float proportion, CancellationToken? cancellationToken) {
            int nEntriesToOverflow = (int)Math.Floor(_NEntriesLastTimeDispatched * proportion);
            Overflow(nEntriesToOverflow, cancellationToken);
        }
        public void Overflow(int nEntriesToOverflow, CancellationToken? cancellationToken)
        {
            if (nEntriesToOverflow <= 0) return;
            if (_OverflowParameters.OverflowToNowhere)
            {
                _OverflowToNowhere(nEntriesToOverflow, cancellationToken);
                return;
            }
            _OverflowToExternal(nEntriesToOverflow, cancellationToken);

        }
        private void _OverflowToNowhere(int nEntriesToOverflow, CancellationToken? cancellationToken) {

            lock (_MapIdentifierToEntry)
            {
                while (nEntriesToOverflow > 0 && _LinkedList.Count > 0)
                {
                    TIdentifier identifier = _LinkedList.First.Value.Item1;
                    _LinkedList.RemoveFirst();
                    _MapIdentifierToEntry.Remove(identifier);
                    nEntriesToOverflow--;
                    if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return;
                }
            }
        }
        public void OverflowAllToExternal() {
            _OverflowToExternal(_LinkedList.Count, null);
        }
        private void _OverflowToExternal(int nEntriesToOverflow, CancellationToken? cancellationToken)
        {
            while (nEntriesToOverflow > 0 && _LinkedList.Count > 0)
            {
                TIdentifier identifier;
                lock (_MapIdentifierToEntry)
                {
                    identifier = _LinkedList.First.Value.Item1;
                }
                _OverflowParameters.ExternalIdentifierLockToOverflow.LockForWrite(identifier, () =>
                {
                    
                    //Not strictly needed because this should always be NoLock if external lock provided for this kind of overflow
                    _IdentifierLock.LockForWrite(identifier, () =>
                    { 
                        lock (_MapIdentifierToEntry)
                        {
                            LinkedListNode<Tuple<TIdentifier, TEntry>> firstNode = _LinkedList.First;
                            if (firstNode == null)
                                return;
                            if (!firstNode.Value.Item1.Equals(identifier))//CHANGED from !==
                                return;
                            _OverflowParameters.OverflowEntry(identifier, firstNode.Value.Item2);
                            _LinkedList.RemoveFirst();
                            _MapIdentifierToEntry.Remove(identifier);
                            nEntriesToOverflow--;
                        }
                    });
                });
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return;
            }
        }
    }
}
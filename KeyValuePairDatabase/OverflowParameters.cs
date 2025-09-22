

namespace KeyValuePairDatabases
{
    public class OverflowParameters<TIdentifier, TEntry>
    {
        private bool _OverflowToNowhere;
        public bool OverflowToNowhere { get { return _OverflowToNowhere; } }
        private IIdentifierLock<TIdentifier> _ExternalIdentifierLockToOverflow;
        public IIdentifierLock<TIdentifier> ExternalIdentifierLockToOverflow { get{return _ExternalIdentifierLockToOverflow;} }
        private Action<TIdentifier, TEntry> _OverflowEntry;
        public Action<TIdentifier, TEntry> OverflowEntry { get { return _OverflowEntry; } }
        public OverflowParameters(
            bool overflowToNowhere, IIdentifierLock<TIdentifier> externalIdentifierLockToOverflow,
            Action<TIdentifier, TEntry> overflowEntry) {
            _OverflowToNowhere = overflowToNowhere;
            _ExternalIdentifierLockToOverflow = externalIdentifierLockToOverflow;
            _OverflowEntry = overflowEntry;
        }
    }
}


namespace KeyValuePairDatabases
{
    public class NEntriesChangedEventArgs : EventArgs
    {
        private int _NEntries;
        public int NEntries { get { return _NEntries; } }
        private int _PreviousNEntries;
        public int PreviousNEntrie { get { return _PreviousNEntries; } }
        public NEntriesChangedEventArgs(int previousNEntries, int nEntries) :base(){
            _PreviousNEntries = previousNEntries;
            _NEntries = nEntries;
        }
    }
}
using Core.FileSystem;
using JSON;

namespace KeyValuePairDatabases.Appended
{
    public class SingleAppendedJsonObjectsFile<TEntry>
    {
        private AppendedJsonObjectsFileHelper<TEntry> _AppendedErrorIdentifiersFileHelper;
        private string _FilePath;
        public SingleAppendedJsonObjectsFile(string filePath, IJsonParser<TEntry> jsonParser, bool throwOnFailParseJsonObject)
        {
            _FilePath = filePath;
            _AppendedErrorIdentifiersFileHelper = new AppendedJsonObjectsFileHelper<TEntry>(jsonParser, throwOnFailParseJsonObject);
        }
        public void Append(TEntry entry) {
            lock (this)
            {
                _AppendedErrorIdentifiersFileHelper.Append(_FilePath, entry);
            }
        }
        public TEntry[] GetNLast(int nEntries, out long nextStartIndexFromBeginningExclusive,
            long? indexToReadFromBackwardsExclusive = null)
        {
            return _AppendedErrorIdentifiersFileHelper.Read(_FilePath, nEntries,
                out nextStartIndexFromBeginningExclusive, indexToReadFromBackwardsExclusive);

        }
    }
}
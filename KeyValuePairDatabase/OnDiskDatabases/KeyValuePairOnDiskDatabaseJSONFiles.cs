using JSON;
using Core.Delegates;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.OnDiskDatabases;

namespace KeyValuePairDatabases
{
    public class KeyValuePairOnDiskDatabaseJSONFiles<TIdentifier, TEntry> : IKeyValuePairOnDiskDatabase<TIdentifier, TEntry>
    {
        private KeyValuePairOnDiskDatabaseJSONFilesStrings<TIdentifier> _KeyValuePairOnDiskDatabaseStrings;
        public KeyValuePairOnDiskDatabaseJSONFiles(string rootDirectory, int nCharactersEachLevel,
            string extension, IIdentifierLock<TIdentifier> identifierLock, Type[] knownTypes)
        {
            //_NativeJsonParser = new Json(knownTypes: knownTypes);
            _KeyValuePairOnDiskDatabaseStrings = new KeyValuePairOnDiskDatabaseJSONFilesStrings<TIdentifier>(rootDirectory, nCharactersEachLevel, extension, identifierLock);
        }
        public bool Has(TIdentifier identifier)
        {
            return _KeyValuePairOnDiskDatabaseStrings.Has(identifier);
        }
        public void ReadCallbackDeleteWithinLock(TIdentifier identifier,
            Action<TEntry> callback)
        {
            _KeyValuePairOnDiskDatabaseStrings.ReadCallbackDeleteWithinLock(identifier, (str) =>
            {
                TEntry entry;
                if (string.IsNullOrEmpty(str)) entry = default(TEntry);
                else entry = Json.Deserialize<TEntry>(str);
                callback(entry);
            });

        }
        public void ReadCallbackWriteWithinLock(TIdentifier identifier, Func<TEntry, TEntry> callback)
        {

            _KeyValuePairOnDiskDatabaseStrings.ReadCallbackWriteWithinLock(identifier, (str) =>
            {
                TEntry entry = string.IsNullOrEmpty(str) ? default(TEntry) : Json.Deserialize<TEntry>(str);
                entry = callback(entry);
                return entry == null ? null : Json.Serialize(entry);
            });
        }
        public void Write(TIdentifier identifier, TEntry entry)
        {

            _KeyValuePairOnDiskDatabaseStrings.Write(identifier, entry == null ? null : Json.Serialize(entry));
        }
        public TEntry ReadWithoutLock(TIdentifier identifier)
        {
            string str = _KeyValuePairOnDiskDatabaseStrings.ReadWithoutLock(identifier);
            return string.IsNullOrEmpty(str) ? default(TEntry) : Json.Deserialize<TEntry>(str);
        }
        public TEntry Read(TIdentifier identifier)
        {

            string str = _KeyValuePairOnDiskDatabaseStrings.Read(identifier);
            return string.IsNullOrEmpty(str) ? default(TEntry) : Json.Deserialize<TEntry>(str);
        }
        public void Delete(TIdentifier identifier)
        {
            _KeyValuePairOnDiskDatabaseStrings.Delete(identifier);
        }
        public void IterateEntries(Action<DelegateNextEntry<TEntry>> callback)
        {
            _KeyValuePairOnDiskDatabaseStrings.IterateEntries((nextPath) => {
                callback((out TEntry entry) =>
                {
                    if (!nextPath(out string filePath))
                    {
                        entry = default(TEntry);
                        return false;
                    }
                    entry = Json.Deserialize<TEntry>(File.ReadAllText(filePath));
                    return true;
                });
            });
        }
    }
}
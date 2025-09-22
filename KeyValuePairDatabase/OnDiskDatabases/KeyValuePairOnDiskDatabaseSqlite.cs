using Core.Delegates;
using KeyValuePairDatabases.Interfaces;
using JSON;

namespace KeyValuePairDatabases.OnDiskDatabases
{
    public class KeyValuePairOnDiskDatabaseSqlite<TIdentifier, TEntry> : IKeyValuePairOnDiskDatabase<TIdentifier, TEntry>
    {
        private KeyValuePairOnDiskDatabaseSqliteStrings<TIdentifier> _Strings;
        public KeyValuePairOnDiskDatabaseSqlite(string rootDirectory, string filePath,
            IIdentifierLock<TIdentifier> identifierLock, int? stringKeyLength= null, Type[] knownTypes = null)
        {
           // _JsonParser = new Json(knownTypes: knownTypes);
            _Strings = new KeyValuePairOnDiskDatabaseSqliteStrings<TIdentifier>(rootDirectory, filePath, identifierLock, stringKeyLength);
        }

        public void Delete(TIdentifier identifier)
        {
            _Strings.Delete(identifier);
        }

        public bool Has(TIdentifier identifier)
        {
            return _Strings.Has(identifier);
        }

        public void IterateEntries(Action<DelegateNextEntry<TEntry>> callback)
        {
            throw new NotImplementedException();
        }

        public TEntry Read(TIdentifier identifier)
        {
            string str = _Strings.Read(identifier);
            return ShouldReturnDefault(str)? default(TEntry) : Json.Deserialize<TEntry>(str);
        }

        public void ReadCallbackDeleteWithinLock(TIdentifier identifier, Action<TEntry> callback)
        {
            _Strings.ReadCallbackDeleteWithinLock(identifier, (str) =>
            {
                TEntry entry = ShouldReturnDefault(str)
                    ?default(TEntry)
                    :Json.Deserialize<TEntry>(str);
                callback(entry);
            });

        }

        public void ReadCallbackWriteWithinLock(TIdentifier identifier, Func<TEntry, TEntry> callback)
        {
            _Strings.ReadCallbackWriteWithinLock(identifier, (str) =>
            {
                TEntry entry = ShouldReturnDefault(str) ? default(TEntry) : Json.Deserialize<TEntry>(str);
                entry = callback(entry);
                return entry == null ? null : Json.Serialize(entry);
            });
        }

        public TEntry ReadWithoutLock(TIdentifier identifier)
        {
            string str = _Strings.ReadWithoutLock(identifier);
            return ShouldReturnDefault(str) ? default(TEntry) : Json.Deserialize<TEntry>(str);
        }

        public void Write(TIdentifier identifier, TEntry entry)
        {
            _Strings.Write(identifier,  entry==null?null:Json.Serialize(entry));
        }
        private bool ShouldReturnDefault(string str) {
            return string.IsNullOrEmpty(str) || str == "null";
        }
    }
}
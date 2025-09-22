
using Core.FileSystem;
using Core.Delegates;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.OnDiskDatabases;
using KeyValuePairDatabases.Enums;

namespace KeyValuePairDatabases
{
    public static class OnDiskDatabaseFactory
    {
        public static IKeyValuePairOnDiskDatabase<TIdentifier, TEntry> Create
                <TIdentifier, TEntry>(OnDiskDatabaseType onDiskDatabaseType, 
                OnDiskDatabaseParams onDiskDatabaseParams, IIdentifierLock<TIdentifier> identifierLock)
        {
            switch (onDiskDatabaseType) {
                case OnDiskDatabaseType.Sqlite:
                    return new KeyValuePairOnDiskDatabaseSqlite<TIdentifier, TEntry>(onDiskDatabaseParams.RootDirectory,
                        onDiskDatabaseParams.FilePath, identifierLock, onDiskDatabaseParams.StringKeyLength);
                case OnDiskDatabaseType.FileSystemJSON:
                return new KeyValuePairOnDiskDatabaseJSONFiles<TIdentifier, TEntry>(
                    onDiskDatabaseParams.RootDirectory,
                    onDiskDatabaseParams.NCharactersEachLevel,
                    onDiskDatabaseParams.Extension,
                    identifierLock,
                    onDiskDatabaseParams.KnownTypes);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
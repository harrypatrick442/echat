using KeyValuePairDatabases;
using Location.Interfaces;
using LocationCore;
using System;
using KeyValuePairDatabases.Interfaces;
using KeyValuePairDatabases.Enums;
namespace LocationDatabase
{
    public class LevelQuadrantsPairsForIdKeyValuePairDatabase : ILevelQuadrantPairsForIdLocalDatabase
    {

        private IIdentifierLock<long> _IdentifierLock;
        private KeyValuePairDatabase<long, LevelQuadrantPairsForId> _LocalDatabase;
        public LevelQuadrantsPairsForIdKeyValuePairDatabase(string rootDirectory){
            _IdentifierLock = new IdentifierLock<long>();
            _LocalDatabase = new KeyValuePairDatabase<long, LevelQuadrantPairsForId>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = rootDirectory,
                    NCharactersEachLevel = 2,
                    Extension = ".json"
                }, new NoIdentifierLock<long>());
        }
        public LevelQuadrantPairsForId Get(long id)
        {
            return _LocalDatabase.Get(id);
        }

        public void LockOnIdForWrite(long id, Action callback)
        {
            _IdentifierLock.LockForWrite(id, callback);
        }

        public void Set(long id, LevelQuadrantPairsForId levelQuadrantPairsForId)
        {
            _LocalDatabase.Set(id, levelQuadrantPairsForId);
        }
    }
}

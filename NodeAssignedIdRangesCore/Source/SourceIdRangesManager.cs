using Core.Exceptions;
using KeyValuePairDatabases;
using NodeAssignedIdRangesCore.Interfaces;
using NodeAssignedIdRangesSource.Serializables;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace NodeAssignedIdRanges
{
    public class SourceIdRangesManager : IIdRangesManager
    {
        //CHECKED
        private static IIdRangesManager? _Instance;
        public static IIdRangesManager Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(SourceIdRangesManager));
            _Instance = new SourceIdRangesManager();
            return _Instance;
        }
        public static IIdRangesManager Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(SourceIdRangesManager));
                return _Instance;
            }
        }
        //CHECKED
        //TODO optimise with volatile
        private Dictionary<int, IdRangesManagerForTypeId> _MapIdTypeToIdRangesManagerForTypeId;
        private KeyValuePairDatabase<long, NextIdFromForIdType> _NextIdFromForIdTypeKeyValuePairDatabase;
        public SourceIdRangesManager()
        {
            _MapIdTypeToIdRangesManagerForTypeId = new Dictionary<int, IdRangesManagerForTypeId>();
            _NextIdFromForIdTypeKeyValuePairDatabase =
                new KeyValuePairDatabase<long, NextIdFromForIdType>(
                    OnDiskDatabaseType.Sqlite,
                    new OnDiskDatabaseParams
                    {
                        RootDirectory = DependencyManager.GetString(DependencyNames.NextIdFromForIdTypeDatabaseDirectory),
                        NCharactersEachLevel = 4,
                        Extension = ".json"
                    }, new IdentifierLock<long>()
            );
        }
        public IIdRangesManagerForIdType ForIdType(int idType)
        {
            lock (_MapIdTypeToIdRangesManagerForTypeId)
            {
                if (_MapIdTypeToIdRangesManagerForTypeId.TryGetValue(idType,
                    out IdRangesManagerForTypeId? idRangesManagerForTypeId))
                {
                    return idRangesManagerForTypeId;
                }
                idRangesManagerForTypeId = new IdRangesManagerForTypeId(idType, 
                    _NextIdFromForIdTypeKeyValuePairDatabase);
                _MapIdTypeToIdRangesManagerForTypeId[idType] = idRangesManagerForTypeId;
                return idRangesManagerForTypeId;
            }
        }
    }
}
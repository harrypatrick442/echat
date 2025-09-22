using Core.Enums;
using Core.Exceptions;
using Chat;
using KeyValuePairDatabases;
using Core.Ids;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace Core.DAL
{
    public class DalPms
    {
        private static DalPms _Instance;
        public static DalPms Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalPms));
                return _Instance;
            }
        }
        public static DalPms Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalPms));
                _Instance = new DalPms();
                return _Instance;
        }

        private KeyValuePairDatabase<string, Pm> _UserIdHighest_LowestToPmKeyValuePairDatabase;
        private KeyValuePairDatabase<long, Pm> _ConversationIdToPmKeyValuePairDatabase;
        private IIdentifierToNodeId<long> _IdentifierToNodeId;
        private DalPms()
        {
            _UserIdHighest_LowestToPmKeyValuePairDatabase
            = new KeyValuePairDatabase<string, Pm>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdHighest_LowestToPmDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension = ".json",
                    StringKeyLength=39
                }, 
                new IdentifierLock<string>());
            _IdentifierToNodeId = Users.UserIdToNodeId.Instance;
            _ConversationIdToPmKeyValuePairDatabase = new KeyValuePairDatabase<long, Pm>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams { 
                    FilePath = DependencyManager.GetString(DependencyNames.ConversationIdToPmDatabaseDirectory)
                },
                new IdentifierLock<long>()
            );
        }
        public Pm GetOrCreatePm(long myUserId, long otherUserId)
        {
            _GetHighestLowestUserIds(myUserId, otherUserId, out long highestUserId, out long lowestUserId);
            Pm pm =  _UserIdHighest_LowestToPmKeyValuePairDatabase.Get(
                _GetIdentifierString(highestUserId, lowestUserId));
             if (pm != null && _ConversationIdToPmKeyValuePairDatabase.HasNotCountingNull(pm.ConversationId))
            {
                return pm;
            }
            DalPms.Instance.ModifyPm(myUserId, otherUserId, (pmInternal, save) => {
                pmInternal = new Pm(myUserId, otherUserId, ConversationIdSource.Instance.NextId());
                save(pmInternal);
                pm = pmInternal;
            });
            return pm;
        }
        public Pm GetPm(long conversationId)
        {
            return _ConversationIdToPmKeyValuePairDatabase.Get(conversationId);
        }
        private void ModifyPm(long myUserId, long otherUserId, 
            Action<Pm, Action<Pm>> callback) {
            _GetHighestLowestUserIds(myUserId, otherUserId, 
                out long highestUserId, out long lowestUserId);
            _UserIdHighest_LowestToPmKeyValuePairDatabase.ModifyWithinLockWithSet(
                _GetIdentifierString(highestUserId, lowestUserId),
                (pm, save)=> { callback(pm, (pmToSave) => { 
                    _ConversationIdToPmKeyValuePairDatabase.Set(pmToSave.ConversationId, pmToSave);
                    save(pmToSave);
                }); }
            );
        }
        private string _GetIdentifierString(long highestUserId, long lowestUserId) {
            return $"{highestUserId}_{lowestUserId}";
        }
        private void _GetHighestLowestUserIds(long myUserId, long otherUserId, out long highestUserId, out long lowestUserId) {
            if (myUserId > otherUserId) {
                highestUserId = myUserId;
                lowestUserId = otherUserId;
                return;
            }
            lowestUserId = myUserId;
            highestUserId = otherUserId; 
        }
    }
}
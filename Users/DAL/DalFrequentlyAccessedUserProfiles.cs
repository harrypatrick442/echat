using Core.Enums;
using Core.Exceptions;
using Users.FrequentlyAccessedUserProfiles;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace Users.DAL
{
    public class DalFrequentlyAccessedUserProfiles
    {
        private static DalFrequentlyAccessedUserProfiles _Instance;
        public static DalFrequentlyAccessedUserProfiles Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalFrequentlyAccessedUserProfiles));
                return _Instance;
            }
        }
        public static DalFrequentlyAccessedUserProfiles Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalFrequentlyAccessedUserProfiles));
                _Instance = new DalFrequentlyAccessedUserProfiles();
                return _Instance;
        }
        private IdentifierLock<long> _IdentifierLock_Here;

        private KeyValuePairDatabaseMesh<long, FrequentlyAccessedUserProfile> _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Core;
        private KeyValuePairInMemoryDatabase<long, FrequentlyAccessedUserProfile> _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Here;
        private DalFrequentlyAccessedUserProfiles()
        {
            _IdentifierLock_Here = new IdentifierLock<long>();
            //TODO IdentifierLock and InMemoryDatabase could use long instead of string in this situation.
            _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Core
                = new KeyValuePairDatabaseMesh<long, FrequentlyAccessedUserProfile>(
                    DatabaseIdentifier.UserIdToFrequentlyAccessedUserProfile.Int(),
                    OnDiskDatabaseType.Sqlite,
                    new OnDiskDatabaseParams
                    {
                        RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToFrequentlyAccessedUserProfileDatabaseDirectory),
                        NCharactersEachLevel = 2,
                        Extension = ".json"
                    }, new IdentifierLock<long>(), UserIdToNodeId.Instance);
            _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Here
                = new KeyValuePairInMemoryDatabase<long, FrequentlyAccessedUserProfile>(
                    new OverflowParameters<long, FrequentlyAccessedUserProfile>(true, null, null),
                    new NoIdentifierLock<long>());
        }
        public FrequentlyAccessedUserProfile GetHereIfHasElseCoreAndCacheHere(long userId)
        {
            FrequentlyAccessedUserProfile frequentlyAccessedUserProfile = null;
            _IdentifierLock_Here.LockForReads(userId, () => {
                frequentlyAccessedUserProfile = _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Here
                    .Get(userId);
            });
            if (frequentlyAccessedUserProfile != null)
                return frequentlyAccessedUserProfile;
            _IdentifierLock_Here.LockForWrite(userId, () => {
                frequentlyAccessedUserProfile = _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Here
                    .Get(userId);
                if (frequentlyAccessedUserProfile != null)
                    return;
                frequentlyAccessedUserProfile = _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Core.Get(userId);
                _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Here.Set(userId, frequentlyAccessedUserProfile);
            });
            return frequentlyAccessedUserProfile;
        }
        public void SetCoreAndHere(long userId, FrequentlyAccessedUserProfile frequentlyAccessedUserProfile)
        {
            string identifierString = userId.ToString();
            _IdentifierLock_Here.LockForWrite(userId, () =>
            {
                _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Core.Set(userId, frequentlyAccessedUserProfile);
                _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Here.Set(userId, frequentlyAccessedUserProfile);
            });
        }
        public void SetHere(long userId, FrequentlyAccessedUserProfile frequentlyAccessedUserProfile)
        {
            _IdentifierLock_Here.LockForWrite(userId, () => {
                _UserIdToDalFrequentlyAccessedUserProfileKeyValuePairDatabase_Here.Set(userId, frequentlyAccessedUserProfile);
            });
        }
    }
}
using Core.Exceptions;
using DependencyManagement;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;

namespace UserIgnore
{
    public class DalUserIgnoresLocal
    {
        private static DalUserIgnoresLocal _Instance;
        public static DalUserIgnoresLocal Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalUserIgnoresLocal));
                return _Instance;
            }
        }
        public static DalUserIgnoresLocal Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalUserIgnoresLocal));
            _Instance = new DalUserIgnoresLocal();
            return _Instance;
        }

        private KeyValuePairDatabase<long, UserIgnores> _UserIdToUserIgnores;
        private KeyValuePairDatabase<long, BeingIgnoredBys> _UserIdToBeingIgnoredBys;
        private DalUserIgnoresLocal()
        {
            _UserIdToUserIgnores
            = new KeyValuePairDatabase<long, UserIgnores>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.UserIdToUserIgnoresDatabaseFilePath)
                }, new IdentifierLock<long>());

            _UserIdToBeingIgnoredBys
            = new KeyValuePairDatabase<long, BeingIgnoredBys>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.UserIdToBeingIgnoredBysDatabaseFilePath)
                }, new IdentifierLock<long>());
        }
        public UserIgnores GetUserIgnores(long userId)
        {
            
            return _UserIdToUserIgnores.Get(userId);
        }
        public void AddUserIgnore(long userIdIgnoring, long userIdBeingIgnored)
        {
            _UserIdToUserIgnores.ModifyWithinLock(userIdIgnoring, (userIgnores) => {
                if (userIgnores == null) userIgnores = new UserIgnores();
                userIgnores.Add(userIdBeingIgnored);
                return userIgnores;
            });
        }
        public void RemoveUserIgnore(long userIdUnignoring, long userIdBeingUnignored)
        {
            _UserIdToUserIgnores.ModifyWithinLock(userIdUnignoring, (userIgnores) => {
                userIgnores?.Remove(userIdBeingUnignored);
                return userIgnores;
            });
        }
        public void AddBeingIgnoredBy(long userIdIgnoring, long userIdBeingIgnored)
        {
            _UserIdToBeingIgnoredBys.ModifyWithinLock(userIdBeingIgnored, (beingIgnoredBys) => {
                if (beingIgnoredBys == null) beingIgnoredBys= new BeingIgnoredBys();
                beingIgnoredBys.Add(userIdIgnoring);
                return beingIgnoredBys;
            });
        }
        public void RemoveBeingIgnoredBy(long userIdUnignoring, long userIdBeingUnignored)
        {
            _UserIdToBeingIgnoredBys.ModifyWithinLock(userIdBeingUnignored, (beingIgnoredBys) => {
                beingIgnoredBys?.Remove(userIdUnignoring);
                return beingIgnoredBys;
            });
        }
    }
}

using Core.Exceptions;
using DependencyManagement;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;
using NotificationsCore.Enums;
using NotificationsCore.Messages;
using NotificationsCore.Messages.Messages;
namespace NotificationsCore
{
    internal class DalNotifications {
        private static DalNotifications? _Instance;
        public static DalNotifications Instance { get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalNotifications));
                return _Instance; } 
        }
        public static DalNotifications Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(DalNotifications));
            _Instance = new DalNotifications();
            return _Instance;
        }
        private KeyValuePairDatabase<long, UserNotifications> _MapUserIdToUserNotifications;
        private DalNotifications() {
            _MapUserIdToUserNotifications = new KeyValuePairDatabase<long, UserNotifications>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams {

                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToUserNotificationsDatabaseFilePath)
                },
                new IdentifierLock<long>()
            );
        }
        public UserNotifications GetUserNotifications(long userId) {
            return _MapUserIdToUserNotifications.Get(userId);
        }
        public bool ClearUpToAtInclusive(long userId, NotificationType type, long upToAtInclusive)
        {
            bool cleared = false;
            _MapUserIdToUserNotifications.ModifyWithinLock(userId, (userNotifications) => {
                if (userNotifications == null)
                    return userNotifications;
                cleared = userNotifications.ClearIfTimeGreater(type, upToAtInclusive);
                return userNotifications;
            });
            return cleared;
        }
        public void SetHasAt(long userId, NotificationType type, long at)
        {
            _MapUserIdToUserNotifications.ModifyWithinLock(userId, (userNotifications) => {
                if (userNotifications == null)
                {
                    userNotifications = new UserNotifications(type, at);
                    return userNotifications;
                }
                userNotifications.SetAt(type, at);
                return userNotifications;
            });
        }
    }
}
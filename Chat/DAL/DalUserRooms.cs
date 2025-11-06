using KeyValuePairDatabases;
using Chat;
using KeyValuePairDatabases.Enums;
using Core.Exceptions;
using DependencyManagement;
using Core.Timing;
using Logging;
using Initialization.Exceptions;
namespace Core.DAL
{
    public class DalUserRooms
    {
        private static DalUserRooms? _Instance;
        public static DalUserRooms Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalUserRooms));
            _Instance = new DalUserRooms();
            return _Instance;
        }
        public static DalUserRooms Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalUserRooms));
                return _Instance;
            }
        }
        KeyValuePairDatabase<long, UserRooms> _MapUserIdToUserRooms;
        protected DalUserRooms()
        {
            _MapUserIdToUserRooms = new KeyValuePairDatabase<long, UserRooms>(OnDiskDatabaseType.Sqlite, new OnDiskDatabaseParams { 
                RootDirectory=DependencyManager.GetString(DependencyNames.UserRoomsDatabaseDirectory)
            }, new IdentifierLock<long>());
        }
        public UserRooms Get(long userId)
        {
            /*CountdownLatch latch = new CountdownLatch(100);
            long start =  TimeHelper.MillisecondsNow;
            for(int i=0; i<100; i++)
                new Thread(() => { try { _MapUserIdToUserRooms.Get(userId); } finally { latch.Signal(); } }).Start();
            latch.Wait();
            long delay = TimeHelper.MillisecondsNow - start;
            Logs.Default.Info("Took " + delay);
            Thread.Sleep(400);*/
            return _MapUserIdToUserRooms.Get(userId);
        }
        public void Modify(long userId, Func<UserRooms, UserRooms> callback)
        {
            _MapUserIdToUserRooms.ModifyWithinLock(userId, callback);
        }
    }
}
using Core.Exceptions;
using Chat;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;
using DependencyManagement;

namespace Core.DAL
{
    public class DalChatRoomInfos
    {
        private static DalChatRoomInfos _Instance;
        public static DalChatRoomInfos Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalChatRoomInfos));
                return _Instance;
            }
        }
        public static DalChatRoomInfos Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalChatRoomInfos));
                _Instance = new DalChatRoomInfos();
                return _Instance;
        }
        private KeyValuePairDatabase<long, ChatRoomInfo> _KeyValuePairDatabaseChatRoomInfos
            = new KeyValuePairDatabase<long, ChatRoomInfo>(
                OnDiskDatabaseType.FileSystemJSON,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.ChatRoomInfosDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension = ".json"
                }, new NoIdentifierLock<long>());
        public void Set(long conversationId, ChatRoomInfo chatRoomInfo)
        {
            _KeyValuePairDatabaseChatRoomInfos.Set(conversationId, chatRoomInfo);
        }
        public ChatRoomInfo Get(long conversationId)
         {
            return _KeyValuePairDatabaseChatRoomInfos.Get(conversationId);
        }
        public void ModifyWithinLock(long conversationId, Func<ChatRoomInfo, ChatRoomInfo> callback) {

            _KeyValuePairDatabaseChatRoomInfos.ModifyWithinLock(conversationId, callback);
        }
        public void ModifyWithinLockWithSet(long conversationId, Action<ChatRoomInfo, Action<ChatRoomInfo>> callback)
        {

            _KeyValuePairDatabaseChatRoomInfos.ModifyWithinLockWithSet(conversationId, callback);
        }
    }
}
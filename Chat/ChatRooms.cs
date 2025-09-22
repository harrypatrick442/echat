using GlobalConstants;
using Core.DAL;
using Core.Exceptions;
using Core.Timing;
using HashTags;
using KeyValuePairDatabases;
using Shutdown;
using System.Timers;
using HashTags.Enums;
using Timer = System.Timers.Timer;
namespace Chat
{
    public sealed class ChatRooms
    {
        private static ChatRooms _Instance;
        public static ChatRooms Initialize() {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(ChatRooms));
            _Instance = new ChatRooms();
            return _Instance;
        }
        public static ChatRooms Instance { 
            get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(ChatRooms));
                return _Instance; 
            }
        }
        private DalChatRoomInfos _DalChatRoomInfos;
        private ChatRooms() {
            _DalChatRoomInfos = DalChatRoomInfos.Instance;
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.ChatRooms);
            _OnlineRecentlySwitchBuffersTimer = new Timer
            {
                Interval = GlobalConstants.Intervals.SWITCH_ONLINE_RECENTLY_BUFFERS,
                AutoReset = true,
                Enabled = true
            };
            _OnlineRecentlySwitchBuffersTimer.Elapsed += SwitchOnlineRecentlyBuffers;
            _OnlineRecentlySwitchBuffersTimer.Start();
        }
        private Dictionary<long, ChatRoom> _MapConversationIdToChatRoom 
            = new Dictionary<long, ChatRoom>();
        private IdentifierLock<long> _RoomIdentifierLock = new IdentifierLock<long>();
        private Timer _OnlineRecentlySwitchBuffersTimer;
        public ChatRoom GetIfExists(long conversationId)
        {
            lock (_MapConversationIdToChatRoom)
            { 
                if(_MapConversationIdToChatRoom.TryGetValue(conversationId, out ChatRoom chatRoom)) {
                    return chatRoom;
                }
            }
            return LoadRoomIfExists(conversationId);
        }
        public ChatRoomInfo Create(string name, long creatorUserId, RoomVisibility? visibility) {
            long conversationId = ConversationIdSource.Instance.NextId();
            ChatRoomInfo chatRoomInfo = new ChatRoomInfo(conversationId, name,
                ConversationHistoryType.FullHistory, creatorUserId, 
                visibility??RoomVisibility.Public);
            _DalChatRoomInfos.Set(conversationId, chatRoomInfo);
            string[] hashTags = HashTagsHelper.SplitStringIntoTags(name)?.ToArray();
            if (hashTags != null)
            {
                HashTagsMesh.Instance.AddTags(hashTags, HashTagScopeTypes.ChatRoom, conversationId, null);
            }
            ChatRoom chatRoom = new ChatRoom(chatRoomInfo);
            lock (_MapConversationIdToChatRoom) {
                _MapConversationIdToChatRoom[conversationId] = chatRoom;
            }
            ChatRoomsMesh.Instance.ModifyUserRooms(creatorUserId, conversationId, true, UserRoomsOperation.Mine, UserRoomsOperation.Joined, UserRoomsOperation.Recent);
            return chatRoomInfo;
        }
        private ChatRoom LoadRoomIfExists(long conversationId)
        {
            ChatRoom chatRoom = null;
            _RoomIdentifierLock.LockForWrite(conversationId, () => {
                lock (_MapConversationIdToChatRoom)
                {
                    if (_MapConversationIdToChatRoom.TryGetValue(conversationId, out chatRoom))
                    {
                        return;
                    }
                }
                ChatRoomInfo chatRoomInfo = _DalChatRoomInfos.Get(conversationId);
                if(chatRoomInfo == null)
                {
                    return;
                }
                bool needsConversationId = chatRoomInfo.ConversationId <= 0;
                if (needsConversationId)
                {
                    _DalChatRoomInfos.ModifyWithinLock(conversationId, (chatRoomInfoToModify) => {
                        chatRoomInfoToModify.ConversationId = conversationId;
                        chatRoomInfo = chatRoomInfoToModify;
                        return chatRoomInfoToModify;
                    });
                }
                chatRoom = new ChatRoom(chatRoomInfo);
                lock (_MapConversationIdToChatRoom) {
                    _MapConversationIdToChatRoom[conversationId] = chatRoom;
                }
            });
            return chatRoom;
        }
        private void SwitchOnlineRecentlyBuffers(object sender, ElapsedEventArgs e)
        {
            ChatRoom[] chatRooms;
            lock (_MapConversationIdToChatRoom)
            {
                chatRooms = _MapConversationIdToChatRoom.Values.ToArray();
            }
            long switchTimeMilliseconds = TimeHelper.MillisecondsNow;
            foreach (ChatRoom chatRoom in chatRooms) {
                chatRoom.SwitchOnlineRecentlyBuffers(switchTimeMilliseconds);
            }
        }
        public void Dispose()
        {
            ChatRoom[] chatRooms;
            _OnlineRecentlySwitchBuffersTimer.Dispose();
            lock (_MapConversationIdToChatRoom)
            {
                chatRooms = _MapConversationIdToChatRoom.Values.ToArray();
            }
            foreach (ChatRoom chatRoom in chatRooms) {
                chatRoom.Dispose();
            }
        }
    }
}
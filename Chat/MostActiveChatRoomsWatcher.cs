using Core.Exceptions;
using KeyValuePairDatabases;
using Shutdown;
using System.Timers;
using Timer = System.Timers.Timer;
using KeyValuePairDatabases.Enums;
using Logging;
using InterserverComs;
using JSON;
using DependencyManagement;
using System.Linq;
using Initialization.Exceptions;

namespace Chat
{
    public sealed class MostActiveChatRoomsWatcher
    {
        private static MostActiveChatRoomsWatcher _Instance;
        public static MostActiveChatRoomsWatcher Initialize(bool isMostActiveChatroomsManager) {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(MostActiveChatRoomsWatcher));
            _Instance = new MostActiveChatRoomsWatcher(isMostActiveChatroomsManager);
            return _Instance;
        }
        public static MostActiveChatRoomsWatcher Instance { 
            get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(MostActiveChatRoomsWatcher));
                return _Instance; 
            }
        }
        private const string MOST_ACTIVE_CHATROOMS_ALL_SERVERS_KEY = "all",
            MOST_ACTIVE_CHATROOMS_THIS_SERVER_KEY = "this";
        private Timer _TimerUpdate;
        private readonly object _LockObjectRoomsWatching = new object();
        private HashSet<ChatRoom> _RoomsWatching = new HashSet<ChatRoom>();
        private readonly object _LockObjectMostActiveRoomsAllServersDescendingCached = new object();
        private RoomActivity[] _MostActiveRoomActivitysForAllNodesDescending;
        private string _MostActiveRoomActivitysForAllNodesDescendingSerialized;
        private IKeyValuePairDatabase<string, MostActiveChatrooms> _MostActiveChatroomsDatabase =
            new KeyValuePairDatabase<string, MostActiveChatrooms>(
                OnDiskDatabaseType.FileSystemJSON,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.ChatRoomsOnThisMachineDirectory),
                    NCharactersEachLevel = 100,
                    Extension = ".json",
                    StringKeyLength= MOST_ACTIVE_CHATROOMS_ALL_SERVERS_KEY.Length
                }, new NoIdentifierLock<string>());
        private MostActiveChatRoomsWatcher(bool isMostActiveChatroomsManager) {
            if (isMostActiveChatroomsManager)
            {

                try
                {
                    MostActiveChatrooms mostActiveChatrooms = _MostActiveChatroomsDatabase.Get(MOST_ACTIVE_CHATROOMS_ALL_SERVERS_KEY);
                    if (mostActiveChatrooms != null)
                    {
                        _MostActiveRoomActivitysForAllNodesDescending = mostActiveChatrooms.Entries;
                        _MostActiveRoomActivitysForAllNodesDescendingSerialized = Json.Serialize(_MostActiveRoomActivitysForAllNodesDescending);
                    }
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                    UpdateAsManager();
                }
                _TimerUpdate = new Timer(Configurations.Intervals.UPDATE_MOST_ACTIVE_ROOMS);
                _TimerUpdate.AutoReset = true;
                _TimerUpdate.Enabled = true;
                _TimerUpdate.Elapsed += ElapsedUpdate;
                _TimerUpdate.Start();
            }
            else {
                try
                {
                    _MostActiveRoomActivitysForAllNodesDescendingSerialized = ChatRoomsMesh.Instance.GetMostActiveRoomsFromManager();
                }
                catch (Exception ex) {
                    Logs.Default.Error(ex);
                }
            }
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.ChatRooms);
        }
        public string GetSerialized()
        {
            lock (_LockObjectMostActiveRoomsAllServersDescendingCached)
            {
                return _MostActiveRoomActivitysForAllNodesDescendingSerialized;
            }
        }
        public void UpdateAsNonManager(MostActiveChatrooms mostActiveRooms) {

            lock (_LockObjectMostActiveRoomsAllServersDescendingCached)
            {
                SetMostActiveRoomActivities(mostActiveRooms?.Entries);
            }
        }
        public void WatchRoomIfElegible(ChatRoom chatRoom) {
            //if (!string.IsNullOrEmpty(chatRoom.Info.Password)) return;
            lock (_LockObjectRoomsWatching) {
                if (_RoomsWatching.Contains(chatRoom)) return;
                _RoomsWatching.Add(chatRoom);
            }
        }
        public string GetMostActiveRoomsAllServers() {
            lock (_LockObjectMostActiveRoomsAllServersDescendingCached)
            {
                if (_MostActiveRoomActivitysForAllNodesDescendingSerialized == null) {
                    UpdateAsManager_NoLock();
                }
                return _MostActiveRoomActivitysForAllNodesDescendingSerialized;
            }
        }
        public RoomActivity[] GetForThisServer(int nMostActive) {
            lock (_LockObjectRoomsWatching)
            {
                IEnumerable<RoomActivity> newEntries = _RoomsWatching
                    .Select(r => r.Info)
                    .OrderByDescending(r => r.NUsers)
                    .Take(nMostActive)
                    .Select(r=>r.ToRoomActivity());
                int nNewEntries = newEntries.Count();
                int nMoreEntriesNeeded = nMostActive - nNewEntries;
                if (nMoreEntriesNeeded>0) {
                    MostActiveChatrooms oldMostActiveChatrooms = _MostActiveChatroomsDatabase
                        .Get(MOST_ACTIVE_CHATROOMS_THIS_SERVER_KEY);
                    if (oldMostActiveChatrooms != null && oldMostActiveChatrooms.Entries != null) {
                        HashSet<long> conversationIdsHave = newEntries.Select(e => e.ConversationId).ToHashSet();
                        newEntries = newEntries
                               .Concat(oldMostActiveChatrooms.Entries.Where(e=> {
                                   if (conversationIdsHave.Contains(e.ConversationId)) return false;
                                   conversationIdsHave.Add(e.ConversationId);
                                   e.NUsers = 0;
                                   return true;
                               }).Take(nMoreEntriesNeeded)).ToArray();
                    }
                }
                RoomActivity[] newRoomActivities = newEntries.ToArray();
                _MostActiveChatroomsDatabase.Set(MOST_ACTIVE_CHATROOMS_THIS_SERVER_KEY, new MostActiveChatrooms(newRoomActivities));
                return newRoomActivities;
            }
        }
        private void ElapsedUpdate(object sender, ElapsedEventArgs e)
        {
            UpdateAsManager();
        }
        private void UpdateAsManager()
        {
            RoomActivity[] mostActiveRooms;
            lock (_LockObjectMostActiveRoomsAllServersDescendingCached)
            {
                UpdateAsManager_NoLock();
                mostActiveRooms = _MostActiveRoomActivitysForAllNodesDescending;
            }
            MostActiveChatrooms mostActiveChatrooms = new MostActiveChatrooms(mostActiveRooms);
            _MostActiveChatroomsDatabase.Set(MOST_ACTIVE_CHATROOMS_ALL_SERVERS_KEY, mostActiveChatrooms);
            OperationRedirectHelper.SendMessageObjectToNodes(mostActiveChatrooms, Configurations.Nodes.Instance.GetNodeIdsAssociatedWithIdType(Configurations.IdTypes.CHAT_ROOM).Where(i=>i!=Nodes.Nodes.Instance.MyId));
        }
        private void UpdateAsManager_NoLock() {
            RoomActivity[] mostActiveRooms = OrderByPriorityAndNUsers(
                ChatRoomsMesh.Instance.GetMostActiveRoomsFromAllRoomServerNodes(Configurations.Lengths.MAX_N_MOST_ACTIVE_CHATROOMS)
            );
            SetMostActiveRoomActivities(mostActiveRooms);
        }
        private void SetMostActiveRoomActivities(RoomActivity[] mostActiveRooms) {
            _MostActiveRoomActivitysForAllNodesDescending = mostActiveRooms;
            _MostActiveRoomActivitysForAllNodesDescendingSerialized = mostActiveRooms == null ? null : Json.Serialize(mostActiveRooms);
        }
        private static RoomActivity[] OrderByPriorityAndNUsers(List<RoomActivity> roomActivities)
        {
            return  roomActivities
                .OrderByDescending(o=>o.NUsers)
            .Take(Configurations.Lengths.MAX_N_MOST_ACTIVE_CHATROOMS)
            .ToArray();
        }
        private void Dispose() {
            _TimerUpdate.Dispose();
        }
    }
}
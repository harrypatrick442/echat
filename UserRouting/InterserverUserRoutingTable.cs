using Core.Exceptions;
using JSON;
using Core.Timing;
using Logging;
using System.Text;
using Core.Handlers;
using Nodes;
using InterserverComs;
using KeyValuePairDatabases;
using MessageTypes.Internal;
using NodeAssignedIdRanges;
using KeyValuePairDatabases.Enums;
using DependencyManagement;
using Core;


namespace UserRouting
{
    public class InterserverUserRoutingTable
    {
        //Core means the machine which carries the object with the users id. 
        private const int TIMEOUT_MILLISECONDS_ADD_REMOVE = 3000,
            TIMEOUT_MILLISECONDS_ADD_REMOVE_CORE=10000,
            MAX_N_TRIES_ADD_ON_OTHER_NODE = 2,
            MAX_USER_IDS_TO_SEND_AT_ONCE=40;
        private bool _DebugLoggingEnabled;
        public delegate long[] DelegateGetEndedSessionIds(long userId, UserRoutingTableEntry userRoutingTableEntry);
        private InterserverPort _InterserverPort { get { return InterserverPort.Instance; } }
        private InterserverEndpoints _InterserverEndpoints { get { return _InterserverPort.InterserverEndpoints; } }
        private KeyValuePairDatabase<long, UserRoutingTableEntry> _KeyValuePairDatabase;
        private Dictionary<long, UserRoutingTableEntry> _MapUserIdToUserRoutingTableEntry_ForNonCoreMachine = new Dictionary<long, UserRoutingTableEntry>();
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private int _MyNodeId;
        private DelegateGetEndedSessionIds _GetEndedSessionIds;
        private ToPushWhenNodesReconnect _ToPushWhenNodesReconnect;
        private Action _HandleRemoveMappings;
        private Nodes.Nodes _Nodes;
        private NodesIdRangesForIdTypeManager _NodesIdRangesForIdTypeManager;
        public InterserverUserRoutingTable(
            DelegateGetEndedSessionIds getEndedSessionIds, bool debugLoggingEnabled)
        {
            _Nodes = Nodes.Nodes.Instance;
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _ToPushWhenNodesReconnect = new ToPushWhenNodesReconnect();
            _GetEndedSessionIds = getEndedSessionIds;
            _NodesIdRangesForIdTypeManager = NodesIdRangesManager.Instance.ForIdType(GlobalConstants.IdTypes.USER);
            _KeyValuePairDatabase = new KeyValuePairDatabase<long, UserRoutingTableEntry>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserRoutingTableDatabaseDirectory),
                    NCharactersEachLevel = 2,
                    Extension = ".json"
                }, new IdentifierLock<long>(), inMemoryOnlyAllowedElseAlwaysWriteToDiskToo: false);
            _HandleRemoveMappings = InterserverMessageTypeMappingsHandler.Instance.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>>
                {
                    { InterserverMessageTypes.UserRoutingMessage, HandleMessage}
                });
            InterserverPort.Instance.InterserverEndpoints.InterserverConnectionOpened +=
                HandleAddedInterserverConnectionOpened;
            _DebugLoggingEnabled = debugLoggingEnabled;

        }
        private void HandleAddedInterserverConnectionOpened(object sender, 
            NodeEndpointEventArgs e)
        {
            new Thread(() =>
            {
                int nodeId = e.NodeEndpoint.NodeId;
                while (_ToPushWhenNodesReconnect.TakeBatchOfUserIds(nodeId,
                    MAX_USER_IDS_TO_SEND_AT_ONCE, out long[] userIds))
                {
                    List<UserRoutingTableEntry> entries = new List<UserRoutingTableEntry>();
                    foreach (long userId in userIds) {
                        UserRoutingTableEntry entry = _KeyValuePairDatabase.Get(userId);
                        if (entry == null)
                            entry = UserRoutingTableEntry.Empty(userId);
                        entries.Add(entry);
                    }
                    try
                    {
                        INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeId);
                        UserRoutingResponseMessage responseMessage = InterserverTicketedSender
                            .Send<UserRoutingMessage, UserRoutingResponseMessage>(
                            new UserRoutingMessage(entries.ToArray(), UserRoutingOperation.BulkNonCoreUpdate, nodeId),
                            TIMEOUT_MILLISECONDS_ADD_REMOVE, 
                            _CancellationTokenSourceDisposed.Token,
                            endpoint.SendJSONString);
                        if (!responseMessage.Success)
                            throw new OperationFailedException("Failed to push range of userId's in bulk update");
                    }
                    catch(Exception ex)
                    {
                        if (_DebugLoggingEnabled)
                        {
                            Logs.Default.Error(ex);
                        }
                        _ToPushWhenNodesReconnect.AddRange(nodeId, userIds);
                    }
                }
            }).Start();
        }

        private void HandleMessage(InterserverMessageEventArgs e)
        {
            UserRoutingMessage userRoutingMessage = Json.Deserialize<UserRoutingMessage>(e.JsonString);
            INodeEndpoint endpointFrom = e.EndpointFrom;
            switch (userRoutingMessage.Operation)
            {
                case UserRoutingOperation.NonCoreUpdate:
                    HandleNonCoreUpdate(endpointFrom, userRoutingMessage);
                    break;
                case UserRoutingOperation.BulkNonCoreUpdate:
                    HandleBulkNonCoreUpdate(endpointFrom, userRoutingMessage);
                    break;
                case UserRoutingOperation.AddCore:
                    HandleAddCore(endpointFrom, userRoutingMessage, endpointFrom.NodeId);
                    break;
                case UserRoutingOperation.RemoveCore:
                    HandleRemoveCore(endpointFrom, userRoutingMessage, endpointFrom.NodeId);
                    break;
                case UserRoutingOperation.GetUserRoutingTableEntry:
                    HandleGetUserRoutingTableEntry(endpointFrom, userRoutingMessage);
                    break;
                case UserRoutingOperation.RemoveAsSessionsNoLongerExistToCoreMachine:
                    RemoveAsLocalSessionsNoLongerExist_ThisIsCoreMachine(endpointFrom.NodeId, userRoutingMessage.UserIdSessionIds);
                    break;
            }
        }
        public NodeIdSessionIdPair[] GetNodeIdSessionIdPairs(long userId)
        {
            int nodeIdForUsersMachine = _NodesIdRangesForIdTypeManager.GetNodeIdForIdInRange(userId);
            UserRoutingTableEntry userRoutingTableEntry = null;
            if (_MyNodeId == nodeIdForUsersMachine)
            {
                userRoutingTableEntry = _KeyValuePairDatabase.Get(userId);
            }
            else {
                lock (_MapUserIdToUserRoutingTableEntry_ForNonCoreMachine)
                {
                    _MapUserIdToUserRoutingTableEntry_ForNonCoreMachine.TryGetValue(userId, out userRoutingTableEntry);
                }
                GetLatestUserRoutingTableEntryIfDisconnectedFromNodeSince(ref userRoutingTableEntry, nodeIdForUsersMachine);
            }
            if (userRoutingTableEntry == null) return null;
            return userRoutingTableEntry.NodeIdSessionIdPairs.ToArray();
        }
        private void HandleGetUserRoutingTableEntry(INodeEndpoint endpointFrom, UserRoutingMessage userRoutingMessage) {
            long userId = userRoutingMessage.UserId;
            UserRoutingTableEntry userRoutingTableEntry = _KeyValuePairDatabase.Get(userId);
            endpointFrom.SendJSONString(Json.Serialize(new UserRoutingResponseMessage(true, userRoutingTableEntry, userRoutingMessage.Ticket)));
        }
        private void GetLatestUserRoutingTableEntryIfDisconnectedFromNodeSince(ref UserRoutingTableEntry userRoutingTableEntry, int nodeIdForUsersMachine) {

            if (userRoutingTableEntry == null) return;
            INodeEndpoint endpoint = _InterserverPort.InterserverEndpoints.GetEndpoint(nodeIdForUsersMachine);
            if (endpoint.ConnectedTimestamp < userRoutingTableEntry.Timestamp) return;
            UserRoutingResponseMessage response = InterserverTicketedSender.Send<UserRoutingMessage, UserRoutingResponseMessage>(
                new UserRoutingMessage(_MyNodeId, userRoutingTableEntry.UserId,
                UserRoutingOperation.GetUserRoutingTableEntry, 0),
                TIMEOUT_MILLISECONDS_ADD_REMOVE_CORE,
                _CancellationTokenSourceDisposed.Token,
                endpoint.SendJSONString);
            userRoutingTableEntry = response.UserRoutingTableEntry;
            if (userRoutingTableEntry!=null)
                NonCoreUpdate(userRoutingTableEntry);
        }
        public NodeAndAssociatedUserIdsSessionIds[] GetNodeAndAssociatedUserIdsSessionIds(long[] userIds, 
            out long[] userIdsRequireForwardingToUserCoreMachine) {
            List<long>userIdsRequireForwardingToUserCoreMachineList = null;
            Dictionary<int, Dictionary<long, List<long>>> _MapNodeIdToMapUserIdToSessionIds = new Dictionary<int, Dictionary<long, List<long>>>();
            foreach (long userId in userIds) {
                NodeIdSessionIdPair[] nodeIdSessionIdPairs = GetNodeIdSessionIdPairs(userId);
                if (nodeIdSessionIdPairs == null) {
                    if (userIdsRequireForwardingToUserCoreMachineList == null)
                        userIdsRequireForwardingToUserCoreMachineList = new List<long> { userId};
                    else userIdsRequireForwardingToUserCoreMachineList.Add(userId);
                    continue;
                }
                foreach (NodeIdSessionIdPair nodeIdSessionIdPair in nodeIdSessionIdPairs) {
                    if (!_MapNodeIdToMapUserIdToSessionIds.TryGetValue(nodeIdSessionIdPair.NodeId, out Dictionary<long, List<long>> mapUserIdToSessionIds))
                    {
                        _MapNodeIdToMapUserIdToSessionIds.Add(nodeIdSessionIdPair.NodeId, new Dictionary<long, List<long>> { { userId, new List<long> { nodeIdSessionIdPair.SessionId } } });
                        continue;
                    }
                    if (!mapUserIdToSessionIds.TryGetValue(userId, out List<long> sessionIds)) {
                        mapUserIdToSessionIds.Add(userId, new List<long> { nodeIdSessionIdPair.SessionId });
                        continue;
                    }
                    sessionIds.Add(nodeIdSessionIdPair.SessionId);
                }
            }
            userIdsRequireForwardingToUserCoreMachine = userIdsRequireForwardingToUserCoreMachineList?.ToArray();
            return _MapNodeIdToMapUserIdToSessionIds.Select(n_U => new NodeAndAssociatedUserIdsSessionIds(n_U.Key, n_U.Value.Select(u_S => new UserIdSessionIds(u_S.Key, u_S.Value.ToArray())).ToArray())).ToArray();

        }
        public void Add(long userId, long sessionId)
        {
            INode node = _NodesIdRangesForIdTypeManager.GetNodeForIdInRange(userId);
            int coreNodeIdForThisUser = node.Id;
            UserRoutingTableEntry userRoutingTableEntry;
            if (_MyNodeId == coreNodeIdForThisUser)
            {
                userRoutingTableEntry = AddHere_Core(_MyNodeId, userId, sessionId);
                UpdateOnOtherNodes(userRoutingTableEntry.GetOtherNodeIds(_MyNodeId), userRoutingTableEntry, _MyNodeId);
                CleanupEndedSessionIds(userId, userRoutingTableEntry);
                return;
            }
            UserRoutingResponseMessage response = InterserverTicketedSender.Send<UserRoutingMessage, UserRoutingResponseMessage>(
                new UserRoutingMessage(_MyNodeId, userId, 
                    UserRoutingOperation.AddCore, sessionId),
                TIMEOUT_MILLISECONDS_ADD_REMOVE_CORE, _CancellationTokenSourceDisposed.Token, 
                GetEndpointForUsersMachine(coreNodeIdForThisUser).SendJSONString);
            NonCoreUpdate(response.UserRoutingTableEntry);
            CleanupEndedSessionIds(userId, response.UserRoutingTableEntry);
            return;
        }
        public void RemoveAsHadNoMappingsForUserId(long userId, long[] sessionIds)
        {
            RemoveAsHadNoMappingsForUserId(new UserIdSessionIds(userId, sessionIds));
        }

        public void Remove(long userId, long sessionId)
        {
            int nodeIdForUsersMachine = _NodesIdRangesForIdTypeManager.GetNodeIdForIdInRange(userId);
            if (_MyNodeId == nodeIdForUsersMachine)
            {
                UserRoutingTableEntry userRoutingTableEntry = RemoveHere_Core(_MyNodeId, userId, sessionId);
                if (userRoutingTableEntry == null) return;
                UpdateOnOtherNodesNotWaitingForResponse(userRoutingTableEntry.GetOtherNodeIds(_MyNodeId), userRoutingTableEntry, _MyNodeId);
                return;
            }
            UserRoutingResponseMessage userRoutingResponseMessage = InterserverTicketedSender.Send<UserRoutingMessage, UserRoutingResponseMessage>(
                new UserRoutingMessage(_MyNodeId, userId,
                    UserRoutingOperation.RemoveCore, sessionId),
                TIMEOUT_MILLISECONDS_ADD_REMOVE_CORE, _CancellationTokenSourceDisposed.Token, 
                GetEndpointForUsersMachine(nodeIdForUsersMachine).SendJSONString);
            if (userRoutingResponseMessage == null) return;
            NonCoreUpdate(userRoutingResponseMessage.UserRoutingTableEntry);
        }
        public void RemoveAsHadNoMappingsForUserId(UserIdSessionIds userIdSessionIds) {
            try
            {
                INode node = _NodesIdRangesForIdTypeManager.GetNodeForIdInRange(userIdSessionIds.UserId);
                if (node.IsMe)
                {
                    RemoveAsLocalSessionsNoLongerExist_ThisIsCoreMachine(node.Id, userIdSessionIds);
                    return;
                }
                UserRoutingMessage message = new UserRoutingMessage(userIdSessionIds,
                    UserRoutingOperation.RemoveAsSessionsNoLongerExistToCoreMachine, _MyNodeId);
                GetEndpointForUsersMachine(node.Id).SendJSONString(
                    Json.Serialize(message));
                foreach (long sessionId in userIdSessionIds.SessionIds) {
                    RemoveHere_NonCore(_MyNodeId, userIdSessionIds.UserId, sessionId);
                }
            }
            catch (Exception ex)
            {
                if (_DebugLoggingEnabled)
                {
                    Logs.Default.Error(ex);
                }
            }
        }
        private void HandleNonCoreUpdate(INodeEndpoint endpoint, UserRoutingMessage userRoutingMessage)
        {
            NonCoreUpdate(userRoutingMessage.UserRoutingTableEntry);
            if (userRoutingMessage.Ticket<=0) return;
            endpoint.SendJSONString(Json.Serialize(new UserRoutingResponseMessage(true, null, userRoutingMessage.Ticket)));
        }
        private void HandleBulkNonCoreUpdate(INodeEndpoint endpoint, UserRoutingMessage userRoutingMessage)
        {//TODO can be fired off more than once possibly caused by this node disconnecting reconnecting as was debugging it
            NonCoreUpdates(userRoutingMessage.UserRoutingTableEntries);
            if (userRoutingMessage.Ticket <= 0) return;
            endpoint.SendJSONString(Json.Serialize(new UserRoutingResponseMessage(true, null, userRoutingMessage.Ticket)));
        }
        private void NonCoreUpdates(UserRoutingTableEntry[] userRoutingTableEntries)
        {
            lock (_MapUserIdToUserRoutingTableEntry_ForNonCoreMachine)
            {
                foreach (UserRoutingTableEntry userRoutingTableEntry in userRoutingTableEntries)
                    NonCoreUpdateInsideLock(userRoutingTableEntry);
                //liberal in that it would rather have entries for sessions no longer live than drop an entry for a live session. Has to be this way or it could fail to forward something to a user endpoint.
            }
        }
        private void NonCoreUpdate(UserRoutingTableEntry userRoutingTableEntry)
        {
            lock (_MapUserIdToUserRoutingTableEntry_ForNonCoreMachine)
            {
                NonCoreUpdateInsideLock(userRoutingTableEntry);
                //liberal in that it would rather have entries for sessions no longer live than drop an entry for a live session. Has to be this way or it could fail to forward something to a user endpoint.
            }
        }
        private void NonCoreUpdateInsideLock(UserRoutingTableEntry userRoutingTableEntry)
        {

            if (userRoutingTableEntry == null)
            {
                Logs.Default.Error($"{nameof(userRoutingTableEntry)} should never be null");
                return;
            }
            long userId = userRoutingTableEntry.UserId;
            bool noEntries = userRoutingTableEntry.NodeIdSessionIdPairs == null || userRoutingTableEntry.NodeIdSessionIdPairs.Count < 1;
            if (!_MapUserIdToUserRoutingTableEntry_ForNonCoreMachine.TryGetValue(userId, out UserRoutingTableEntry existing))
            {
                if (noEntries)
                    return;
                _MapUserIdToUserRoutingTableEntry_ForNonCoreMachine[userId] = userRoutingTableEntry;
                return;
            }
            if (noEntries)
            {
                if (userRoutingTableEntry.Timestamp < existing.Timestamp)
                    return;
                _MapUserIdToUserRoutingTableEntry_ForNonCoreMachine.Remove(userId);
                return;
            }
            if (userRoutingTableEntry.Timestamp > existing.Timestamp)
            {
                _MapUserIdToUserRoutingTableEntry_ForNonCoreMachine[userId] = userRoutingTableEntry;
                return;
            }
            existing.AddNodeIdSessionIdPairsIfDoesntHave(userRoutingTableEntry);
        }
        private void RemoveAsLocalSessionsNoLongerExist_ThisIsCoreMachine(int nodeId, UserIdSessionIds userIdSessionIds) {
            UserRoutingTableEntry userRoutingTableEntry = null;
            _KeyValuePairDatabase.ModifyWithinLock(userIdSessionIds.UserId, (userRoutingTableEntryInner) => {
                userRoutingTableEntry = userRoutingTableEntryInner;
                if (userRoutingTableEntryInner == null) return userRoutingTableEntryInner;
                bool delete = false;
                foreach (long sessionId in userIdSessionIds.SessionIds)
                {
                    delete = userRoutingTableEntryInner.Remove(nodeId, sessionId);
                }
                return delete ?null: userRoutingTableEntryInner;
            });
            if (userRoutingTableEntry == null) return;
            int[] nodeIds = userRoutingTableEntry.GetOtherNodeIds(new int[] { _MyNodeId, nodeId });
            UpdateOnOtherNodesNotWaitingForResponse(nodeIds, userRoutingTableEntry, nodeId);
        }
        private void HandleRemoveCore(INodeEndpoint endpointFrom,
            UserRoutingMessage userRoutingMessage, int nodeIdToSkip)
        {
            UserRoutingTableEntry userRoutingTableEntry = RemoveHere_Core(userRoutingMessage.NodeId, userRoutingMessage.UserId, userRoutingMessage.SessionId);
            if (userRoutingTableEntry != null)
            {
                UpdateOnOtherNodesNotWaitingForResponse(userRoutingTableEntry.GetOtherNodeIds(_MyNodeId), userRoutingTableEntry, nodeIdToSkip);
            }
            endpointFrom.SendJSONString(Json.Serialize(new UserRoutingResponseMessage(true, userRoutingTableEntry, userRoutingMessage.Ticket)));
        }
        private void HandleAddCore(INodeEndpoint endpointFrom, UserRoutingMessage userRoutingMessage, int nodeIdToSkip)
        {
            UserRoutingTableEntry userRoutingTableEntry = AddHere_Core(userRoutingMessage.NodeId, userRoutingMessage.UserId, userRoutingMessage.SessionId);

            UpdateOnOtherNodes(userRoutingTableEntry.GetOtherNodeIds(_MyNodeId),userRoutingTableEntry , nodeIdToSkip);
            endpointFrom.SendJSONString(Json.Serialize(new UserRoutingResponseMessage(true, userRoutingTableEntry, userRoutingMessage.Ticket)));
        }
        private UserRoutingTableEntry AddHere_Core(int nodeId, long userId, long sessionId)
        {
            UserRoutingTableEntry userRoutingTableEntryOut = null;

            _KeyValuePairDatabase.ModifyWithinLock(userId, (userRoutingTableEntry) =>
            {
                if (userRoutingTableEntry == null)
                {
                    userRoutingTableEntryOut = (userRoutingTableEntry = new UserRoutingTableEntry(userId, new NodeIdSessionIdPair(nodeId, sessionId)));
                    userRoutingTableEntry.Timestamp = TimeHelper.MillisecondsNow;
                    return userRoutingTableEntry;
                }
                userRoutingTableEntry.Timestamp = TimeHelper.MillisecondsNow;
                userRoutingTableEntryOut = userRoutingTableEntry;
                userRoutingTableEntry.Add(nodeId, sessionId);
                return userRoutingTableEntry;
            });
            return userRoutingTableEntryOut;
        }
        private void UpdateOnOtherNodesNotWaitingForResponse(
            IEnumerable<int> otherNodeIds, UserRoutingTableEntry userRoutingTableEntry, int nodeIdToSkip)
        {
            foreach (int otherNodeId in otherNodeIds)
            {
                if (otherNodeId == nodeIdToSkip || otherNodeId == _MyNodeId) continue;
                INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(otherNodeId);
                if (endpoint == null)
                {
                    _ToPushWhenNodesReconnect.Add(otherNodeId, userRoutingTableEntry.UserId);
                    continue;
                }
                try
                {
                    endpoint.SendJSONString(Json.Serialize(new UserRoutingMessage(UserRoutingOperation.NonCoreUpdate, userRoutingTableEntry)));
                }
                catch (Exception ex)
                {
                    if (_DebugLoggingEnabled)
                    {
                        Logs.Default.Error(ex);
                    }
                    _ToPushWhenNodesReconnect.Add(otherNodeId, userRoutingTableEntry.UserId);
                }
            }
        }
        private void UpdateOnOtherNodes(IEnumerable<int> otherNodeIds,
            UserRoutingTableEntry userRoutingTableEntry,
            int nodeIdToSkip)
        {
            foreach (int otherNodeId in otherNodeIds)
            {
                if (otherNodeId == nodeIdToSkip || otherNodeId == _MyNodeId) continue;
                INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(otherNodeId);
                if (endpoint == null)
                {
                    _ToPushWhenNodesReconnect.Add(otherNodeId, userRoutingTableEntry.UserId);
                    continue;
                }
                try
                {
                    UserRoutingResponseMessage responseMessage = InterserverTicketedSender.Send<UserRoutingMessage, UserRoutingResponseMessage>(
                        new UserRoutingMessage(UserRoutingOperation.NonCoreUpdate, userRoutingTableEntry),
                        TIMEOUT_MILLISECONDS_ADD_REMOVE, _CancellationTokenSourceDisposed.Token, endpoint.SendJSONString);
                }
                catch (Exception ex)
                {
                    if (_DebugLoggingEnabled)
                    {
                        Logs.Default.Error(ex);
                    }
                    _ToPushWhenNodesReconnect.Add(otherNodeId, userRoutingTableEntry.UserId);
                }
            }
        }
        private UserRoutingTableEntry RemoveHere_NonCore(int nodeId, long userId, long sessionId)
        {
            UserRoutingTableEntry userRoutingTableEntry;
            lock (_MapUserIdToUserRoutingTableEntry_ForNonCoreMachine)
            {
                if (!_MapUserIdToUserRoutingTableEntry_ForNonCoreMachine.TryGetValue(userId, out userRoutingTableEntry)) return null;
                bool delete = userRoutingTableEntry.Remove(nodeId, sessionId);
                if(delete)
                    _MapUserIdToUserRoutingTableEntry_ForNonCoreMachine.Remove(userId);
            }
            return userRoutingTableEntry;
        }
        private UserRoutingTableEntry RemoveHere_Core(int nodeId, long userId, long sessionId) {

            UserRoutingTableEntry userRoutingTableEntryOut = null;
            _KeyValuePairDatabase.ModifyWithinLock(userId, (userRoutingTableEntry) => {
                if (userRoutingTableEntry == null) return null;
                userRoutingTableEntry.Timestamp = TimeHelper.MillisecondsNow;
                userRoutingTableEntryOut = userRoutingTableEntry;
                bool delete = userRoutingTableEntry.Remove(nodeId, sessionId);
                return delete?null:userRoutingTableEntry;

            });
            return userRoutingTableEntryOut;
        }
        private INodeEndpoint GetEndpointForUsersMachine(int nodeIdForUsersMachine) {
            INodeEndpoint endpoint = _InterserverEndpoints.GetEndpoint(nodeIdForUsersMachine);
            if (endpoint == null) throw new OperationFailedException("The node with id {nodeId} associated with the user had no established connection to this node");
            return endpoint;
        }
        #region Debugging
        public string GetInMemorySnapshot() {
            StringBuilder sb = new StringBuilder(); 
            sb.AppendLine("[");
            bool firstOuter = true;
            lock (_MapUserIdToUserRoutingTableEntry_ForNonCoreMachine)
            {
                foreach (KeyValuePair<long, UserRoutingTableEntry> keyValuePair in _MapUserIdToUserRoutingTableEntry_ForNonCoreMachine)
                {
                    if (firstOuter) firstOuter = false;
                    else sb.Append(",");
                    sb.Append("{\"i\":");
                    sb.Append(keyValuePair.Key.ToString());
                    sb.Append(",\"n\":[");
                    bool first = true;
                    foreach (NodeIdSessionIdPair nodeIdSessionIds in keyValuePair.Value.NodeIdSessionIdPairs)
                    {

                        if (first) first = false;
                        else sb.Append(",");
                        sb.Append("{\"nI\":");
                        sb.Append(nodeIdSessionIds.NodeId);
                        sb.Append(",\"sI\":");
                        sb.Append(nodeIdSessionIds.SessionId);
                        sb.Append("}");

                    }
                    sb.AppendLine("]}");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
        public string GetOnDiskSnapshot()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append("[");
            _KeyValuePairDatabase.IterateEntries((next) => {
                while (next(out UserRoutingTableEntry userRoutingTableEntry))
                {
                    if (first) first = false;
                    else sb.Append(",");
                    sb.Append(Json.Serialize(userRoutingTableEntry));
                }
            });
            sb.Append("]");
            return sb.ToString();
        }
        #endregion
        private void CleanupEndedSessionIds(long userId, UserRoutingTableEntry userRoutingTableEntry)
        {
            long[] sessionIdsNoLongerHas = _GetEndedSessionIds(userId, userRoutingTableEntry);
            if (sessionIdsNoLongerHas.Length < 1) return;
            RemoveAsHadNoMappingsForUserId(
                new UserIdSessionIds(userId, sessionIdsNoLongerHas)
            );
        }

        public virtual void Dispose()
        {
            _CancellationTokenSourceDisposed.Cancel();
            Action handleRemoveMappings = _HandleRemoveMappings;
            handleRemoveMappings?.Invoke();
        }
    }
}
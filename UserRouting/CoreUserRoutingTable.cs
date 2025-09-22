using UserRouting;
using Core.Exceptions;
using Shutdown;
using Nodes;
using Core.Interfaces;

namespace UserRouting
{
    public class CoreUserRoutingTable: IShutdownable, IUserRoutingTable<IClientEndpoint>
    {
        private static CoreUserRoutingTable _Instance;
        public static CoreUserRoutingTable Instance { 
            get {
                if (_Instance == null) throw new NotInitializedException(nameof(CoreUserRoutingTable));
                return _Instance;
            } 
        }

        public ShutdownOrder ShutdownOrder => ShutdownOrder.SnippetsUserRoutingTable;
        private InterserverUserRoutingTable _UserRoutingTable;
        private LocalUserRoutingTable<IClientEndpoint> _LocalUserRoutingTable;
        public static CoreUserRoutingTable Initialize(bool debugLoggingEnabled) {
            if (_Instance!=null) throw new AlreadyInitializedException(nameof(CoreUserRoutingTable));
            _Instance = new CoreUserRoutingTable(debugLoggingEnabled);
            return _Instance;
        }
        public string GetInMemorySnapshot() {
            return _UserRoutingTable.GetInMemorySnapshot();
        }
        public string GetOnDiskSnapshot() {
            return _UserRoutingTable.GetOnDiskSnapshot();
        }
        private CoreUserRoutingTable(bool debugLoggingEnabled) {
            _UserRoutingTable = new InterserverUserRoutingTable(GetEndedSessionIds, debugLoggingEnabled);
            _LocalUserRoutingTable = new LocalUserRoutingTable<IClientEndpoint>();
        }
        public void Add(long userId, long sessionId, IClientEndpoint endpoint)
        {
            _LocalUserRoutingTable.Add(userId, sessionId, endpoint);
            _UserRoutingTable.Add(userId, sessionId);
        }
        public void Remove(long userId, long sessionId)
        {
            _UserRoutingTable.Remove(userId, sessionId);
            _LocalUserRoutingTable.Remove(userId, sessionId);
        }
        public IClientEndpoint[] GetEndpointsForUserIds(IEnumerable<long> userIds, out long[] userIdsDidntHave) {
            return _LocalUserRoutingTable.GetEndpointsForUserIds(userIds, out userIdsDidntHave);
        }
        public Dictionary<long, IClientEndpoint> GetMapSessionIdToEndpointForUserId_Editable(long userId)
        {
            return _LocalUserRoutingTable.GetMapSessionIdToEndpointForUserId(userId);
        }
        public IClientEndpoint GetLocalEndpoint(long userId, long sessionId)
        {
            return _LocalUserRoutingTable.GetEndpoint(userId, sessionId);
        }
        public IClientEndpoint[] GetLocalEndpointsForUser(long userId)
        {
            return _LocalUserRoutingTable.GetLocalEndpointsForUser(userId);
        }

        public void RemoveAsHadNoMappingsForUserId(UserIdSessionIds userIdSessionIds)
        {
            _UserRoutingTable.RemoveAsHadNoMappingsForUserId(userIdSessionIds);
        }
        public NodeAndAssociatedUserIdsSessionIds[] GetNodeAndAssociatedUserIdsSessionIds(long[] userIds,
            out long[] userIdsRequireForwardingToUserCoreMachine)
        {
            return _UserRoutingTable.GetNodeAndAssociatedUserIdsSessionIds(userIds, out userIdsRequireForwardingToUserCoreMachine);
        }
        public NodeIdSessionIdPair[] GetNodeIdSessionIdPairs(long userId) {
            return _UserRoutingTable.GetNodeIdSessionIdPairs(userId);
        }
        public int? GetNodeIdForUserIdSessionId(long userId, long sessionId) {
            return _UserRoutingTable.GetNodeIdSessionIdPairs(userId)?
                .Where(n=>n.SessionId == sessionId)
                .FirstOrDefault()?.NodeId;
        }
        private long[] GetEndedSessionIds(long userId, UserRoutingTableEntry userRoutingTableEntry)
        {
            long[] sessionIdsNoLongerHas = _LocalUserRoutingTable.GetSessionIdsNoLongerHasForUser(userId, 
                userRoutingTableEntry.GetSessionIdsForNode(Nodes.Nodes.Instance.MyId));
            return sessionIdsNoLongerHas;
        }
        public void Dispose()
        {
            _UserRoutingTable.Dispose();
        }
    }
}

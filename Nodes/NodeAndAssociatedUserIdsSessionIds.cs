namespace Nodes
{
    public class NodeAndAssociatedUserIdsSessionIds
    {
        private int _NodeId;
        public int NodeId { get { return _NodeId; } }
        private UserIdSessionIds[] _UserIdSessionIdss;
        public UserIdSessionIds[] UserIdSessionIdss { get { return _UserIdSessionIdss; } }
        public NodeAndAssociatedUserIdsSessionIds(int nodeId, UserIdSessionIds[] userIdSessionIdss) {
            _NodeId = nodeId;
            _UserIdSessionIdss = userIdSessionIdss;
        }

    }
}

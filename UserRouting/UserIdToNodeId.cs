using Core.Ids;
using NodeAssignedIdRanges;
using Core.Exceptions;

namespace Users
{
    public sealed class UserIdToNodeId : IIdentifierToNodeId<long>
    {
        private static UserIdToNodeId _Instance;
        public static UserIdToNodeId Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(UserIdToNodeId));
            _Instance = new UserIdToNodeId();
            return _Instance;
        }
        public static UserIdToNodeId Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(UserIdToNodeId));
                return _Instance;
            }
        }

        public int[] AllNodesIds => _NodesIdRangesForIdTypeManager.AllNodeIds;

        NodesIdRangesForIdTypeManager _NodesIdRangesForIdTypeManager;
        public UserIdToNodeId() {

            _NodesIdRangesForIdTypeManager = NodesIdRangesManager.Instance.ForIdType(GlobalConstants.IdTypes.USER);
        }
        public int GetNodeIdFromIdentifier(long identifier)
        {
            return _NodesIdRangesForIdTypeManager.GetNodeIdForIdInRange(identifier);
        }
    }
}
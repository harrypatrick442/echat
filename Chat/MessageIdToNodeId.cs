using Core.Ids;
using NodeAssignedIdRanges;
using Core.Exceptions;

namespace Chat
{
    public sealed class MessageIdToNodeId : IIdentifierToNodeId<long>
    {
        private static MessageIdToNodeId _Instance;
        public static MessageIdToNodeId Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MessageIdToNodeId));
            _Instance = new MessageIdToNodeId();
            return _Instance;
        }
        public static MessageIdToNodeId Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(MessageIdToNodeId));
                return _Instance;
            }
        }

        public int[] AllNodesIds => _NodesIdRangesForIdTypeManager.AllNodeIds;

        NodesIdRangesForIdTypeManager _NodesIdRangesForIdTypeManager;
        public MessageIdToNodeId()
        {

            _NodesIdRangesForIdTypeManager = NodesIdRangesManager.Instance.ForIdType(GlobalConstants.IdTypes.MESSAGE);
        }
        public int GetNodeIdFromIdentifier(long identifier)
        {
            return _NodesIdRangesForIdTypeManager.GetNodeIdForIdInRange(identifier);
        }
    }
}
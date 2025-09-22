using Core.Ids;
using NodeAssignedIdRanges;
using Core.Exceptions;

namespace Chat
{
    public sealed class ConversationIdToNodeId : IIdentifierToNodeId<long>
    {
        private static ConversationIdToNodeId _Instance;
        public static ConversationIdToNodeId Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(ConversationIdToNodeId));
            _Instance = new ConversationIdToNodeId();
            return _Instance;
        }
        public static ConversationIdToNodeId Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(ConversationIdToNodeId));
                return _Instance;
            }
        }

        public int[] AllNodesIds => _NodesIdRangesForIdTypeManager.AllNodeIds;

        NodesIdRangesForIdTypeManager _NodesIdRangesForIdTypeManager;
        public ConversationIdToNodeId() {

            _NodesIdRangesForIdTypeManager = NodesIdRangesManager.Instance.ForIdType(GlobalConstants.IdTypes.CONVERSATION);
        }
        public int GetNodeIdFromIdentifier(long identifier)
        {
            return _NodesIdRangesForIdTypeManager.GetNodeIdForIdInRange(identifier);
        }
        public List<NodeIdAndAssociatedIds> GetNodeIds(long[] conversationIds, List<long> missingConversationIds = null)
        {

            return _NodesIdRangesForIdTypeManager.GetNodeIdsForIdsInRange(conversationIds, missingConversationIds);
        }
        public IEnumerable<NodeIdAndAssociattedObjects<TObject>> GetNodeIdAndAssociatedObjects_s
            <TObject>(IEnumerable<TObject> objects, Func<TObject, long> getObjectIdentifier, List<long> missingConversationIds = null)
        {

            return _NodesIdRangesForIdTypeManager.GetNodeIdAndAssociatedObjects_s(objects, getObjectIdentifier, missingConversationIds);
        }
    }
}
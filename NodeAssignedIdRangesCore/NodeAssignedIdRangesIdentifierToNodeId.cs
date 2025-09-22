
using Core.Exceptions;
using Core.Ids;
using NodeAssignedIdRanges;

namespace Nodes.Ids
{
    public class NodeAssignedIdRangesIdentifierToNodeId : IdentifierToNodeId
    {
        private static NodeAssignedIdRangesIdentifierToNodeId _Instance;
        public static NodeAssignedIdRangesIdentifierToNodeId Initialize(INodes nodes, int idType) {
            if (_Instance != null) 
                throw new AlreadyInitializedException(nameof(NodeAssignedIdRangesIdentifierToNodeId));
            _Instance = new NodeAssignedIdRangesIdentifierToNodeId(nodes, idType);
            return _Instance;
        }
        public static NodeAssignedIdRangesIdentifierToNodeId Instance { get
            {
                if (_Instance == null) 
                    throw new NotInitializedException(nameof(NodeAssignedIdRangesIdentifierToNodeId));
                return _Instance;
            } 
        }
        private INodes _Nodes;
        public NodeAssignedIdRangesIdentifierToNodeId(INodes nodes, int idType) {
            _Nodes = nodes;
            MyAssociatedNodesIdRangesForIdType.
        }
        public override int GetNodeIdFromStringIdentifier(string identifier) 
        {
            return MyAssociatedNodesIn.GetNodeForIdInRange(long.Parse(identifier)).Id;
        }
        public override int GetNodeIdFromLongIdentifier(long identifier)
        {
            return _Nodes.GetNodeForIdInRange(identifier).Id;
        }
    }
}
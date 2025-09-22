
using Nodes;

namespace NodeAssignedIdRanges
{
    public class NodeIdAndAssociatedIds
    {
        //CHECKED
        public int NodeId { get; }
        public long[] Ids { get; }
        public NodeIdAndAssociatedIds(int nodeId, long[] ids) {
            NodeId= nodeId;
            Ids = ids;
        }

    }
}


using Nodes;

namespace NodeAssignedIdRanges
{
    public class NodeAndAssociatedIds
    {
        //CHECKED
        public INode Node { get; }
        public long[] Ids { get; }
        public NodeAndAssociatedIds(INode node, long[] ids) {
            Node = node;
            Ids = ids;
        }

    }
}

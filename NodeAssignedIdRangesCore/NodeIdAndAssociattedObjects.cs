
using Nodes;

namespace NodeAssignedIdRanges
{
    public class NodeIdAndAssociattedObjects<TObject>
    {
        public int NodeId { get; }
        public TObject[] Objects { get; }
        public NodeIdAndAssociattedObjects(int nodeId, TObject[] objects) {
            NodeId= nodeId;
            Objects = objects;
        }

    }
}

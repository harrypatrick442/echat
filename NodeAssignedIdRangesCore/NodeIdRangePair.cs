namespace NodeAssignedIdRanges
{
    public class NodeIdRangePair
    {
        //CHECKED
        public int NodeId{ get; protected set; }
        public IdRange IdRange { get; protected set; }
        public NodeIdRangePair(int nodeId, IdRange idRange)
        {
            NodeId = nodeId;
            IdRange = idRange;
        }
    }
}
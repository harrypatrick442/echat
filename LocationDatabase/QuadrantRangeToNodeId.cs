namespace Location
{
    public struct QuadrantRangeToNodeId
    {
        public long ToExclusive { get; }
        public int NodeId {  get; }
        public QuadrantRangeToNodeId(long toExclusive, int nodeId) { 
            ToExclusive = toExclusive;
            NodeId = nodeId;
        }
    }
}

namespace LocationCore
{
    public struct NodeIdAndQuadrants
    {
        public int NodeId { get; }
        public long[] Quadrants{ get; }
        public NodeIdAndQuadrants(int nodeId, long[] quadrants) {
            NodeId = nodeId;
            Quadrants = quadrants;
        }
    }
}

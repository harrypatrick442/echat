namespace LocationCore
{
    public struct NodeIdAndLevelQuadrantPairs
    {
        public int NodeId { get; }
        public LevelQuadrantPair[] LevelQuadrantPairs{ get; }
        public NodeIdAndLevelQuadrantPairs(int nodeId, LevelQuadrantPair[] levelQuadrantPairs) {
            NodeId = nodeId;
            LevelQuadrantPairs = levelQuadrantPairs;
        }
    }
}

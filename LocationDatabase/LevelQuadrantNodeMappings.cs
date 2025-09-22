using System;

namespace LocationDatabase
{
    public class LevelQuadrantNodeMappings
    {
        private QuadrantRangeToNodeId[][] _LevelToQuadrantRangeToNodeId;
        public int NLevels { get; }
        public LevelQuadrantNodeMappings(int[][] nodeIdsAtEachLevel) {
            NLevels = nodeIdsAtEachLevel.Length;
            _LevelToQuadrantRangeToNodeId = new QuadrantRangeToNodeId[NLevels][];
            long nQuads = 4;
            for (int level = 0; level < NLevels; level++)
            {
                int[] nodeIds = nodeIdsAtEachLevel[level];
                long nQuadsPerNode = nQuads / nodeIds.Length;
                long toExclusive = nQuads;
                QuadrantRangeToNodeId[] quadrantRangeToNodeArray = new QuadrantRangeToNodeId[nodeIds.Length];
                int j = nodeIds.Length-1;
                while(j>=0) {
                    quadrantRangeToNodeArray[j] = new QuadrantRangeToNodeId(toExclusive, nodeIds[j]);
                    toExclusive -= nQuadsPerNode;
                    j--;
                }
                nQuads *= 4;
                _LevelToQuadrantRangeToNodeId[level] = quadrantRangeToNodeArray;
            }
        }
        public int GetNodeId(int level, long quadrant) {
            QuadrantRangeToNodeId[] ranges = _LevelToQuadrantRangeToNodeId[level];
            QuadrantRangeToNodeId range;
            int i = 0;
            while (i < ranges.Length)
            {
                range = ranges[i++];
                if (quadrant < range.ToExclusive)
                {
                    return range.NodeId;
                }
            }
            throw new IndexOutOfRangeException();
        }
    }
}

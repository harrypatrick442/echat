using GlobalConstants;
namespace HashTags
{
    public struct HashTagLocalMappingWithSegments
    {
        public HashTagsNodeShardMapping Mapping { get; }
        public string SegmentsJoined { get; }
        public HashTagLocalMappingWithSegments(HashTagsNodeShardMapping endMapping, string segmentsJoined) { 
            Mapping = endMapping;
            SegmentsJoined = segmentsJoined;
        }
    }
}
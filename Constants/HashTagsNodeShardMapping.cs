namespace GlobalConstants
{
    public class HashTagsNodeShardMapping
    {
        public int NodeId { get; }
        public char[] Chars { get; }
        public HashTagsNodeShardMapping[] Children { get; }
        public HashTagsNodeShardMapping(char[] chars, int nodeId)
        {
            Chars = chars;
            NodeId = nodeId;
        }
        public HashTagsNodeShardMapping(char c, int nodeId, HashTagsNodeShardMapping[]? children = null)
        {
            Chars = new char[] { c };
            NodeId = nodeId;
            Children = children;
        }
        public HashTagsNodeShardMapping(int nodeId, HashTagsNodeShardMapping[] children)
        {
            NodeId = nodeId;
            Children = children;
        }
    }
}
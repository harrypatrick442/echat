using Database;
namespace HashTags
{
    internal class ActivatedNodeShardMapping
    {
        /// <summary>
        /// If the node is at the end then the combined string of characters leading to it 
        /// forms the shard name. It will be located on the node with the nodeId specified on the end node.
        /// </summary> 
        public int NodeId { get; }
        public LocalSQLite? Shard { get; }
        private Dictionary<char, ActivatedNodeShardMapping>? _MapCharToChild;
        public ActivatedNodeShardMapping GetNode(string tag, int index)
        {
            if (index >= tag.Length)
            {
                return this;
            }
            if (_MapCharToChild == null)
                return this;
            char c = tag[index];
            if (!_MapCharToChild.TryGetValue(c, out ActivatedNodeShardMapping? childNode))
                return this;
            index++;
            return childNode!.GetNode(tag, index);
        }
        public ActivatedNodeShardMapping(
            LocalSQLite? shard, Dictionary<char, ActivatedNodeShardMapping>? mapCharToChild, 
            int nodeId)
        {
            Shard = shard;
            NodeId = nodeId;
            _MapCharToChild = mapCharToChild;
        }
    }
}
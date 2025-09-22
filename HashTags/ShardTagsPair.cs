using Database;

namespace HashTags
{
    public struct ShardTagsPair
    {
        public LocalSQLite Shard
        {
            get;
        }
        public string[] Tags { get; }
        public ShardTagsPair(LocalSQLite shard, string[] tags)
        {
            Shard = shard;
            Tags = tags;
        }
    }
}
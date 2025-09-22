namespace HashTags
{
    public struct NodeIdAndAssociatedTags
    {
        public int NodeId { get; }
        public string[] Tags { get;}
        public NodeIdAndAssociatedTags(int nodeId, string[] tags)
        {
            NodeId = nodeId;
            Tags = tags;
        }
    }
}
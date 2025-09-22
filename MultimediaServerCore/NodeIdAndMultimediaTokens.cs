namespace MultimediaServerCore
{
    public struct NodeIdAndMultimediaTokens
    {
        public string[] MultimediaTokens { get; }
        public int NodeId { get; }
        public NodeIdAndMultimediaTokens(int nodeId, string[] multimediaTokens) { 
            MultimediaTokens = multimediaTokens;
            NodeId = nodeId;
        }
    }
}
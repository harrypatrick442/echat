namespace Nodes
{
    public class NodeEventArgs : EventArgs
    {
        private INode _Node;
        public INode Node { get { return _Node; } }
        public NodeEventArgs(INode node) : base()
        {
            _Node = node;
        }
    }
}
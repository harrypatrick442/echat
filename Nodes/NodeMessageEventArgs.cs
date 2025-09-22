using Core.Events;

namespace Nodes
{
    public class NodeMessageEventArgs : MessageEventArgs
    {
        private INode _Node;
        public INode Node { get { return _Node; } }
        public NodeMessageEventArgs(INode node, string message) : base(message)
        {
            _Node = node;
        }
    }
}
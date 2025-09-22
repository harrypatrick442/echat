namespace Nodes
{
    public interface INodes
    {
        INode GetNodeById(int identifier);
        INode Me { get; }
    }
}
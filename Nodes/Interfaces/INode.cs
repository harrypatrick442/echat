namespace Nodes
{
    public interface INode
    {
        int Id { get; }
        bool IsMe { get; }
        InterserverConnection[] InterserverConnections { get; }
        //event EventHandler<ItemEventArgs<InterserverConnection>> OnInterserverConnectionAdded;
        //event EventHandler<ItemEventArgs<long>> OnInterserverConnectionRemoved;
        InterserverConnection GetInterserverConnectionTo(int toNodeId);
        int[] AssociatedIdTypes { get; }

    }
}
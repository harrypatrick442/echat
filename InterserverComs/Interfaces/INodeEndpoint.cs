using Nodes;
namespace InterserverComs
{
    public interface INodeEndpoint
    {
        event EventHandler<NodeEndpointEventArgs> OnOpened;
        event EventHandler<NodeEndpointEventArgs> OnClosed;
        public bool IsOpen { get; }
        long InstanceId { get; }
        int NodeId { get; }
        long ConnectedTimestamp { get; }
        void SendJSONString(string jsonString);
        // event EventHandler<NodeMessageEventArgs> OnMessage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newInterserverConnection"></param>
        /// <returns>needsRecreating</returns>
        NodeEndpointState NodeEndpointState { get; }
        void Dispose();
    }
}
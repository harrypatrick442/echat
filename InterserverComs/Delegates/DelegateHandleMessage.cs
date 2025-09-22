using Nodes;
namespace InterserverComs.Delegates
{
    public delegate void DelegateHandleMessage(INodeEndpoint endpointFrom, string jsonString);
}
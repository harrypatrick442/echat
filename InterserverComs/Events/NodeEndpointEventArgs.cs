using Core.Interfaces;
using System;

namespace InterserverComs
{
    public class NodeEndpointEventArgs: EventArgs
    {
        private INodeEndpoint _NodeEndpoint;
        public INodeEndpoint NodeEndpoint { get { return _NodeEndpoint; } }
        public NodeEndpointEventArgs(INodeEndpoint nodeEndpoint) {
            _NodeEndpoint = nodeEndpoint;
        }
    }
}
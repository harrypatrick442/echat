namespace Nodes
{
    public static class NodeEndpointStatesHistory
    {
        private static Dictionary<long, NodeEndpointState> _MapInstanceIdentifierToState = new Dictionary<long, NodeEndpointState>();
        public static void Add(NodeEndpointState state) {
            lock (_MapInstanceIdentifierToState) {
                _MapInstanceIdentifierToState[state.InstanceId]= state;
            }
        }
        public static void Closed(long instanceId)
        {
            lock (_MapInstanceIdentifierToState)
            {
                if (_MapInstanceIdentifierToState.TryGetValue(instanceId, out NodeEndpointState state))
                {
                    state.Closed();
                }
            }
        }
        public static void CloseEvent(long instanceId)
        {
            lock (_MapInstanceIdentifierToState)
            {
                if (_MapInstanceIdentifierToState.TryGetValue(instanceId, out NodeEndpointState state))
                {
                    state.CloseEvent();
                }
            }
        }
        public static void Opened(long instanceId)
        {
            lock (_MapInstanceIdentifierToState)
            {
                if (_MapInstanceIdentifierToState.TryGetValue(instanceId, out NodeEndpointState state))
                {
                    state.Opened();
                }
                else { }
            }
        }
        public static void OpenEvent(long instanceId)
        {
            lock (_MapInstanceIdentifierToState)
            {
                if (_MapInstanceIdentifierToState.TryGetValue(instanceId, out NodeEndpointState state))
                {
                    state.OpenEvent();
                }
            }
        }
        public static void AccessEntriesInLock(Action<NodeEndpointState[]> callback)
        {
            lock (_MapInstanceIdentifierToState)
            {
                NodeEndpointState[] states = _MapInstanceIdentifierToState.Values.ToArray();
                callback(states);
            }
        }

    }
}

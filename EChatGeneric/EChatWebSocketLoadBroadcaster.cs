using EChatEndpoints.WebsocketServers;
using Core.LoadBalancing;
using Core.Exceptions;
namespace EChatGeneric
{
    public class EChatWebSocketLoadBroadcaster:LoadingBroadcaster
    {
        private static EChatWebSocketLoadBroadcaster _Instance;
        public static EChatWebSocketLoadBroadcaster Initialize() { 
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(EChatWebSocketLoadBroadcaster));
                _Instance = new EChatWebSocketLoadBroadcaster();
                return _Instance;
        }
        private EChatWebSocketLoadBroadcaster():base(GlobalConstants.Nodes.ECHAT_FILESERVER_NODES,
            LoadFactorType.EChatWebsocketServer, ()=>EChatUserWebsocketServer.NInstances){ 
        
        }
    }
}
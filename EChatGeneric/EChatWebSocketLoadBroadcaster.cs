using EChatEndpoints.WebsocketServers;
using Core.Exceptions;
using WebAbstract.LoadBalancing;
using ConfigurationCore;
using Initialization.Exceptions;
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
        private EChatWebSocketLoadBroadcaster():base(Configurations.Nodes.ECHAT_FILESERVER_NODES,
            LoadFactorType.EChatWebsocketServer, ()=>EChatUserWebsocketServer.NInstances){ 
        
        }
    }
}
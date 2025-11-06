using Initialization.Exceptions;
using WebAbstract.LoadBalancing;

namespace MultimediaServerCore
{
    public sealed class MultimediaServerLoadBalancer : ReceivingLoadBalancerBase
    {
        private static MultimediaServerLoadBalancer _Instance;
        public static MultimediaServerLoadBalancer Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MultimediaServerLoadBalancer));
            _Instance = new MultimediaServerLoadBalancer();
            return _Instance;
        }

        private readonly object _LockObjectNodeIdsLoadBalancingArray = new object();
        private int[] _NodeIdsLoadBalancingArray;
        private int _CurrentIndex;
        public int GetNextNodeId()
        {
#if DEBUG
                return Configurations.Nodes.MULTIMEDIA_SERVER_DEBUG;
#else
                if(_CurrentIndex>= _NodeIdsLoadBalancingArray.Length)
                    _CurrentIndex = 0;
                return _NodeIdsLoadBalancingArray[_CurrentIndex++];
#endif
        }
        protected override void NewNodeIdsEstablished(int[] nodeIds)
        {
            lock (_LockObjectNodeIdsLoadBalancingArray) {
                _NodeIdsLoadBalancingArray = nodeIds;
            }
        }

        public static MultimediaServerLoadBalancer Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(MultimediaServerLoadBalancer));
                return _Instance;
            }
        }

        private MultimediaServerLoadBalancer() :base(LoadFactorType.MultimediaServer,
            Configurations.Intervals.MULTIMEDIA_SERVER_LOAD_BALANCER_UPDATE)
        {
            _NodeIdsLoadBalancingArray = Configurations.Nodes.ECHAT_MULTIMEDIA_SERVER_NODES;
            ReceivingLoadBalancer.Instance.AddHandler(this);
        }
    }
}
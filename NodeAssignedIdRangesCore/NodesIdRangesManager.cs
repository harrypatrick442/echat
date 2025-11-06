using ConfigurationCore;
using Core.Exceptions;
using DependencyManagement;
using Initialization.Exceptions;
using InterserverComs;
using Logging;
using NodeAssignedIdRangesCore.Requests;
using Nodes;
using Shutdown;

namespace NodeAssignedIdRanges
{
    public sealed class NodesIdRangesManager
    {
        private const int TIMEOUT_POPULATE = 3*60 * 1000;
        private readonly object _LockObjectDispose = new object();
        private bool _Disposed = false;
        private int _IdServerNodeId =

#if DEBUG
            Configurations.Nodes.ID_SERVER_NODE_ID_DEBUG
#else
                Configurations.Nodes.ID_SERVER_NODE_ID
#endif
            ;
        private int _MyNodeId;
        //CHECKED
        private static NodesIdRangesManager? _Instance;
        public static NodesIdRangesManager Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(NodesIdRangesManager));
            _Instance = new NodesIdRangesManager();
            return _Instance;
        }
        public static NodesIdRangesManager Instance
        {
            get
            {
                if (_Instance == null) 
                    throw new NotInitializedException(nameof(NodesIdRangesManager));
                return _Instance;
            }
        }
        private  Dictionary<int, NodesIdRangesForIdTypeManager> _ForIdType = new Dictionary<int, NodesIdRangesForIdTypeManager>();
        private NodesIdRangesManager()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.IdRangesMesh);
            if (Nodes.Nodes.Instance.MyId != _IdServerNodeId)
            {
                InterserverPort.Instance.InterserverEndpoints.InterserverConnectionOpened 
                    += IdServerEndpointOpenedOrReopened;
            }
            Populate();
        }
        private void IdServerEndpointOpenedOrReopened(object sender, NodeEndpointEventArgs e)
        {
            if (e.NodeEndpoint.NodeId == _IdServerNodeId)
            {
                new Thread(()=>Populate()).Start();
            }
        }
        private void Populate() {
            try
            {
                lock (_ForIdType)
                {
                    NodesIdRangesForIdType[] nodesIdRangesForIdTypes = _IdServerNodeId==_MyNodeId 
                        ? IdRangesMesh.Instance.GetNodesIdRangesForAllAssociatedIdTypes_Here(_MyNodeId)
                        :IdRangesMesh.Instance.GetNodesIdRangesForAllAssociatedIdTypesUsingAjax(_MyNodeId, TIMEOUT_POPULATE);
                    foreach (NodesIdRangesForIdType nodesIdRangesForIdType in nodesIdRangesForIdTypes)
                    {
                        int idType = nodesIdRangesForIdType.IdType;
                        NodesIdRangesForIdTypeManager forIdTypeManager = new NodesIdRangesForIdTypeManager(
                            nodesIdRangesForIdType.IdType, nodesIdRangesForIdType.NodeIdRangess);
                        _ForIdType[idType] = forIdTypeManager;
                    }
                }
            }   
            catch (Exception ex)
            {
                var fEx = new FatalException("Failed to get node id ranges for all associated id types", ex);
                Logs.HighPriority.Error(fEx);
                ShutdownManager.Instance.Shutdown(exitCode: 2);
            }
        }
        internal void AnotherNodeGotNewIdRange(int idType, int nodeId, IdRange range) {
            ForIdType(idType).AnotherNodeGotNewIdRange(nodeId, range);
        }
        public INode GetNodeForId(int idType, long id)
        {
            return ForIdType(idType).GetNodeForIdInRange(id);
        }
        public NodesIdRangesForIdTypeManager ForIdType(int idType) {
            lock (_ForIdType)
            {
                if (!_ForIdType.TryGetValue(idType, out NodesIdRangesForIdTypeManager? forIdType))
                {
                    throw new FatalException($"Something went very wrong. There was no existing {nameof(NodesIdRangesForIdTypeManager)} in {nameof(_ForIdType)} for {nameof(idType)} {idType}");
                }
                return forIdType;
            }
        }
        public MyIdRangesForIdType MineForIdType(int idType) {
            return ForIdType(idType).MyIdRangesForIdType;
        }
        private void Dispose() {
            lock (_LockObjectDispose)
            {
                if (_Disposed) return;
                _Disposed = true;
                InterserverPort.Instance.InterserverEndpoints.InterserverConnectionOpened -= IdServerEndpointOpenedOrReopened;
            }
        }
    }
}

using Core.Exceptions;
using Shutdown;
using InterserverComs;
using Logging;
using NodeAssignedIdRangesCore.Requests;
using NodeAssignedIdRangesCore.Responses;
using Core.Handlers;
using Core;
using MessageTypes.Internal;
using Ajax;
using JSON;
using System.Net.Http;

namespace NodeAssignedIdRanges
{
    public sealed partial class IdRangesMesh
    {
        private static IdRangesMesh? _Instance;
        public static IdRangesMesh Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(IdRangesMesh));
            _Instance = new IdRangesMesh();
            return _Instance;
        }
        public static IdRangesMesh Instance {
            get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(IdRangesMesh));
                return _Instance;
            }
        }
        private int _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private IdRangesMesh() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.IdsGiveMeNewIdRange, HandleGiveMeNewIdRange},
                    {InterserverMessageTypes.IdAnotherServerGotANewIdRange, HandleAnotherServerGotANewIdRange},
                    { InterserverMessageTypes.IdGetNodesIdRangesForAllAssociatedIdTypes, HandleGetNodesIdRangesForAllAssociatedIdTypes}
                }
            );
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.IdRangesMesh);
        }
        #region Methods
        #region Public
        public NodesIdRangesForIdType[] GetNodesIdRangesForAllAssociatedIdTypesUsingAjax(int nodeId, int timeoutMilliseconds) {

            string url = $"https://{GlobalConstants.Nodes.FirstUniqueDomainForNode(GlobalConstants.Nodes.
                    ID_SERVER_NODE_ID)}/{GlobalConstants.RoutesWithoutSlash.ID_SERVER_GET}?nodeId={nodeId}";
            AjaxResult result = AjaxHelper.Get(url, Json.Instance, null, timeoutMilliseconds);
            Logs.Default.Info("Did get ");
            if (!result.Successful) {
                throw new OperationFailedException($"Failed doing AJAX with status code {result.StatusCode}", result.AjaxException);
            }
            Logs.Default.Info("Was successful");
            Logs.Default.Info(result.GetRawPayload());
            return result.GetJSONPayload< NodesIdRangesForIdType[]>(timeoutMilliseconds);
        }
        public NodesIdRangesForIdType[] GetNodesIdRangesForAllAssociatedIdTypes()
        {
            NodesIdRangesForIdType[]? nodesIdRangesForIdTypes = null;
            OperationRedirectHelper.OperationRedirectedToNode<
                GetNodesIdRangesForAllAssociatedIdTypesRequest,
                GetNodesIdRangesForAllAssociatedIdTypesResponse>(
#if DEBUG
                GlobalConstants.Nodes.
                ID_SERVER_NODE_ID_DEBUG
#else
                GlobalConstants.Nodes.ID_SERVER_NODE_ID
#endif
                ,
                () =>
                {
                    nodesIdRangesForIdTypes =
                        GetNodesIdRangesForAllAssociatedIdTypes_Here(Nodes.Nodes.Instance.MyId);
                },
                () => new GetNodesIdRangesForAllAssociatedIdTypesRequest(),
                (response) =>
                {
                    if (!response.Success)
                        throw new OperationFailedException($"Failed to get node id ranges for associated id types for node {Nodes.Nodes.Instance.MyId}");
                    nodesIdRangesForIdTypes = response.NodesIdRangesForIdTypes!;
                },
                _CancellationTokenSourceDisposed.Token
            );
            return nodesIdRangesForIdTypes!;
        }
        public IdRange GiveMeNewIdRange(int typeId)
        {
            int myNodeId = Nodes.Nodes.Instance.MyId;
            IdRange? newNodeIdRange = null;
            OperationRedirectHelper.OperationRedirectedToNode<
                GiveMeNewIdRangeRequest,
                GiveMeNewIdRangeResponse>(

#if DEBUG
                GlobalConstants.Nodes.ID_SERVER_NODE_ID_DEBUG
#else
                GlobalConstants.Nodes.ID_SERVER_NODE_ID
#endif
                ,
                () =>
                {
                    newNodeIdRange = GiveMeNewIdRange_Here(typeId, myNodeId);
                },
                () => new GiveMeNewIdRangeRequest(typeId),
                (response) =>
                {
                    newNodeIdRange = response.NodeIdRange;
                },
                _CancellationTokenSourceDisposed.Token
            );
            return newNodeIdRange!;
        }
#endregion Public
        #region Private
        private void SendAnotherServerGotANewIdRangeToOtherNodesConnected(int idType, 
            int nodeIdAssignedTo, IdRange newIdRange)
        {
            IEnumerable<int> otherNodeIds =
                GlobalConstants.Nodes.GetNodeIdsAssociatedWithIdType(idType)
                .Where(i=>i != nodeIdAssignedTo);
            foreach (int otherNodeId in otherNodeIds)
            {
                new Thread(() =>
                {
                    try
                    {
                        SendAnotherServerGotANewIdRangeToOtherNodeConnected(
                            otherNodeId, idType, nodeIdAssignedTo, newIdRange,
                            _CancellationTokenSourceDisposed.Token);
                    }
                    catch (Exception ex)
                    {
                        Logs.HighPriority.Error(ex);
                        ShutdownManager.Instance.Shutdown(exitCode: 2);
                    }
                }).Start();
            }
        }
        private void SendAnotherServerGotANewIdRangeToOtherNodeConnected(
            int otherNodeId, int idType, int nodeIdAssignedTo,
            IdRange newIdRange, CancellationToken cancellationToken)
        {

            TryUpToNTimes(MAX_N_ATTEMPTS_SEND_TO_NODE, () => {
                bool understood = false;
                OperationRedirectHelper.OperationRedirectedToNode<
                    AnotherServerGotANewIdRangeRequest,
                    AcknowledgeResponse>(otherNodeId,
                    () =>{
                        try
                        {
                            AnotherServerGotANewIdRange_Here(idType, nodeIdAssignedTo, newIdRange);
                            understood = true;
                        }
                        catch (Exception ex)
                        {
                            Logs.HighPriority.Error(ex);
                            ShutdownManager.Instance.Shutdown(exitCode: 2);
                        }
                    },
                    () => new AnotherServerGotANewIdRangeRequest(idType, nodeIdAssignedTo, newIdRange),
                    (response) =>
                    {
                        understood = response.Understood;
                    },
                    cancellationToken
                );
                if (!understood)
                    throw new OperationFailedException($"Failed to send a new id range to node {otherNodeId}");
            });
        }
        private void TryUpToNTimes(int nTimes, Action callback)
        {
            Exception? exception = null;
            for (var i = 0; i < nTimes; i++)
            {
                try
                {
                    callback();
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            if (exception != null)
                throw exception;
        }
        private void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
        }
        #endregion Private
#endregion Methods
    }
}
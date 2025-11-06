using Core.Exceptions;
using Shutdown;
using FileInfo = Core.Messages.Messages.FileInfo;
using InterserverComs;
using Logging;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Requests;
using MultimediaServerCore.Messages;
using JSON;
using Core.Messages.Responses;
using Core.Threading;
using Initialization.Exceptions;

namespace MultimediaServerCore
{
    public partial class MultimediaServerMesh
    {
        private static MultimediaServerMesh _Instance;
        public static MultimediaServerMesh Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MultimediaServerMesh));
            _Instance = new MultimediaServerMesh();
            return _Instance;
        }
        public static MultimediaServerMesh Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(MultimediaServerMesh));
                return _Instance;
            }
        }
        private long _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private MultimediaServerMesh()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.MultimediaServerMesh);

        }
        #region Methods
        #region Public
        public MultimediaFailedReason? PrepareToUpload(
            MultimediaType multimediaType, FileInfo fileInfo, MultimediaScopeType scopeType, long scopingId,
            long? scopingId2, long? scopingId3, out string? multimediaToken)
        {
            string? multimediaTokenInternal = null;
            try
            {
                int nodeId = MultimediaServerLoadBalancer.Instance
                    .GetNextNodeId();
                MultimediaFailedReason? failedReasonInternal = null;
                OperationRedirectHelper.OperationRedirectedToNode<
                    PrepareToUploadRequest,
                    PrepareToUploadResponse>(nodeId,
                    () =>
                    {
                        PrepareToUpload_Here(Nodes.Nodes.Instance.MyId, multimediaType, fileInfo, 
                            scopeType, scopingId, scopingId2, scopingId3, out failedReasonInternal, out multimediaTokenInternal);
                    },
                    () => new PrepareToUploadRequest(Nodes.Nodes.Instance.MyId, multimediaType, fileInfo,
                        scopeType, scopingId, scopingId2, scopingId3),
                    (response) =>
                    {
                        failedReasonInternal = response.FailedReason;
                        multimediaTokenInternal = response.MultimediaToken;
                    },
                    _CancellationTokenSourceDisposed.Token
                );
                multimediaToken = multimediaTokenInternal;
                return failedReasonInternal;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            multimediaToken = null;
            return MultimediaFailedReason.ServerError;
        }
        public void MultimediaUploadUpdate(int nodeIdRequestingUpload,
            MultimediaStatusUpdate multimediaUploadUpdate)
        {
            try
            {
                if (nodeIdRequestingUpload == _MyNodeId)
                {
                    MultimediaUploadUpdate_Here(multimediaUploadUpdate);
                    return;
                }
                INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(nodeIdRequestingUpload);
                if (nodeEndpoint == null)
                    return;
                nodeEndpoint.SendJSONString(Json.Serialize(multimediaUploadUpdate));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        public void Delete(params string[] rawMultimediaTokens)
        {
            Delete((IEnumerable<string>)rawMultimediaTokens);
        }
        public void Delete(IEnumerable<string> rawMultimediaTokens) {
           
            IEnumerable<NodeIdAndMultimediaTokens> nodeIdAndMultimediaTokenss = rawMultimediaTokens
                .Select(m=>new { multimediaToken=m, nodeId= MultimediaTokenHelper.GetNodeId(m) })
                .GroupBy(o=>o.nodeId)
                .Select(g=>new NodeIdAndMultimediaTokens(
                    g.First().nodeId,
                    g.Select(o=>o.multimediaToken).ToArray()
                    )
               );
            try
            {
                ParallelOperationHelper.RunInParallelNoReturn(nodeIdAndMultimediaTokenss, (nodeIdAndMultimediaTokens) =>
                    {
                        OperationRedirectHelper.OperationRedirectedToNode<DeleteMultimediaRequest, SuccessTicketedResponse>(
                            nodeIdAndMultimediaTokens.NodeId,
                            () => Delete_Here(nodeIdAndMultimediaTokens.MultimediaTokens),
                            () => new DeleteMultimediaRequest(nodeIdAndMultimediaTokens.MultimediaTokens),
                            (response) =>
                            {
                                if (!response.Success)
                                    throw new Exception($"Something went wrong deleting the multimediaToken \"{nodeIdAndMultimediaTokens.MultimediaTokens}\"");
                            },
                            _CancellationTokenSourceDisposed.Token
                        );
                    },
                    Configurations.Threading.MAX_N_THREADS_MULTIMEDIA_ITEMS_DELETE_PARALLEL_OPERATION,
                    throwExceptions:true
                );
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        #endregion Public
        #region Private
        private void Dispose()
        {
            _CancellationTokenSourceDisposed.Cancel();
        }
        #endregion
        #endregion Methods
    }
}
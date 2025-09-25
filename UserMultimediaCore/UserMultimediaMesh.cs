using Core.Exceptions;
using Shutdown;
using InterserverComs;
using Logging;
using MultimediaServerCore.Requests;
using Nodes;
using JSON;
using UserRouting;
using UserMultimediaCore.Requests;

namespace UserMultimediaCore
{
    public partial class UserMultimediaMesh
    {
        private static UserMultimediaMesh? _Instance;
        public static UserMultimediaMesh Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(UserMultimediaMesh));
            _Instance = new UserMultimediaMesh();
            return _Instance;
        }
        public static UserMultimediaMesh Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(UserMultimediaMesh));
                return _Instance;
            }
        }
        private long _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private UserMultimediaMesh()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.MultimediaServerMesh);

        }
        #region Methods
        #region Public
        public void PushUserMultimediaMetadataUpdateToUserEndpoints(long userId, string userMultimediaMetadataUpdateJsonString)
        {
            //Only sends to the user uploading atm. Its expected cached user objects in a clients browser will periodically update anyway.
            NodeAndAssociatedUserIdsSessionIds[] nodeAndAssociatedUserIdsSessionIdss =
                CoreUserRoutingTable.Instance.GetNodeAndAssociatedUserIdsSessionIds(
                new long[] { userId }, out long[] userIdsRequireForwarding);
            foreach (NodeAndAssociatedUserIdsSessionIds n in nodeAndAssociatedUserIdsSessionIdss)
            {
                try
                {
                    if (n.NodeId == _MyNodeId)
                    {
                        PushUserMultimediaMetadataUpdateToUserEndpoints_Here(n.UserIdSessionIdss, userMultimediaMetadataUpdateJsonString);
                        continue;
                    }
                    INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(n.NodeId);
                    if (nodeEndpoint == null)
                        continue;
                    nodeEndpoint.SendJSONString(Json.Serialize(
                        new PushUserMultimediaMetadataUpdateToUserEndpoints(
                            n.UserIdSessionIdss, userMultimediaMetadataUpdateJsonString)
                        )
                   );
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }
        }
        public void PushUserMultimediaDelete(MultimediaDelete userMultimediaDelete)
        {
            //Only sends to the user uploading atm. Its expected cached user objects in a clients browser will periodically update anyway.
            NodeAndAssociatedUserIdsSessionIds[] nodeAndAssociatedUserIdsSessionIdss =
                CoreUserRoutingTable.Instance.GetNodeAndAssociatedUserIdsSessionIds(
                new long[] { userMultimediaDelete.UserId }, out long[] userIdsRequireForwarding);
            string userMultimediaDeleteJsonString = Json.Serialize(userMultimediaDelete);
            foreach (NodeAndAssociatedUserIdsSessionIds n in nodeAndAssociatedUserIdsSessionIdss)
            {
                try
                {
                    if (n.NodeId == _MyNodeId)
                    {
                        PushUserMultimediaDeleteToUserEndpoints_Here(n.UserIdSessionIdss, userMultimediaDeleteJsonString);
                        continue;
                    }
                    INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(n.NodeId);
                    if (nodeEndpoint == null)
                        continue;
                    nodeEndpoint.SendJSONString(Json.Serialize(
                        new PushUserMultimediaDeleteToUserEndpoints(
                            n.UserIdSessionIdss, userMultimediaDeleteJsonString)
                        )
                   );
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
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
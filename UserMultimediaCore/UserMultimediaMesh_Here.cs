using Core.Interfaces;
using Core.Threading;
using Logging;
using Nodes;
using UserRouting;

namespace UserMultimediaCore
{
    public partial class UserMultimediaMesh
    {
        private void PushMultimediaUploadToUserEndpoints_Here(
            UserIdSessionIds[] userIdSessionIds,
            string userMultimediaUploadJsonObject)
        {
            try
            {
                IClientEndpoint[] clientEndpoints = CoreUserRoutingTable.Instance.GetEndpointsForUserIds(userIdSessionIds.Select(u => u.UserId).ToArray(), out long[] didntHave);
                ParallelOperationHelper.RunInParallelNoReturn(clientEndpoints, (clientEndpoint) =>
                {
                    try
                    {
                        clientEndpoint.SendJSONString(userMultimediaUploadJsonObject);
                    }
                    catch (Exception ex)
                    {
                        Logs.Default.Error(ex);
                    }
                }, Configurations.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_USERS_DEVICES_HERE);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void PushUserMultimediaMetadataUpdateToUserEndpoints_Here(
            UserIdSessionIds[] userIdSessionIds,
            string userMultimediaMetadataUpdateJsonObject)
        {
            try
            {
                IClientEndpoint[] clientEndpoints = CoreUserRoutingTable.Instance.GetEndpointsForUserIds(userIdSessionIds.Select(u => u.UserId).ToArray(), out long[] didntHave);
                ParallelOperationHelper.RunInParallelNoReturn(clientEndpoints, (clientEndpoint) =>
                {
                    try
                    {
                        clientEndpoint.SendJSONString(userMultimediaMetadataUpdateJsonObject);
                    }
                    catch (Exception ex)
                    {
                        Logs.Default.Error(ex);
                    }
                }, Configurations.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_USERS_DEVICES_HERE);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void PushUserMultimediaDeleteToUserEndpoints_Here(
            UserIdSessionIds[] userIdSessionIds,
            string userMultimediaDeleteJsonObject)
        {
            try
            {
                IClientEndpoint[] clientEndpoints = CoreUserRoutingTable.Instance.GetEndpointsForUserIds(userIdSessionIds.Select(u => u.UserId).ToArray(), out long[] didntHave);
                ParallelOperationHelper.RunInParallelNoReturn(clientEndpoints, (clientEndpoint) =>
                {
                    try
                    {
                        clientEndpoint.SendJSONString(userMultimediaDeleteJsonObject);
                    }
                    catch (Exception ex)
                    {
                        Logs.Default.Error(ex);
                    }
                }, Configurations.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_USERS_DEVICES_HERE);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}
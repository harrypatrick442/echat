using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Core;
using MultimediaServerCore.Requests;
using MultimediaServerCore.Messages;
using UserMultimediaCore.Messages;
using UserMultimediaCore.Requests;

namespace UserMultimediaCore
{
    public partial class UserMultimediaMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    { InterserverMessageTypes.UserMultimediaPushToUserEndpoints , HandlePushUserMultimediaUploadToUserEndpoints},
                    { InterserverMessageTypes.UserMultimediaPushMetadataUpdateToUserEndpoints , HandlePushUserMultimediaMetadataUpdateToUserEndpointsUpdate},
                    { InterserverMessageTypes.UserMultimediaPushDeleteToUserEndpoints, HandlePushUserMultimediaDeleteToUserEndpointsUpdate}
                }
            );
        }

        private void HandlePushUserMultimediaUploadToUserEndpoints(InterserverMessageEventArgs e)
        {
            try
            {
                PushMultimediaUploadToUserEndpoints request = e.Deserialize<PushMultimediaUploadToUserEndpoints>();
                PushMultimediaUploadToUserEndpoints_Here(request.UserIdSessionIdss, request.SuccessfulMultimediaUploadJsonString);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandlePushUserMultimediaMetadataUpdateToUserEndpointsUpdate(InterserverMessageEventArgs e)
        {
            try
            {
                PushUserMultimediaMetadataUpdateToUserEndpoints request = e.Deserialize<PushUserMultimediaMetadataUpdateToUserEndpoints>();
                PushUserMultimediaMetadataUpdateToUserEndpoints_Here(request.UserIdSessionIdss, request.UserMultimediaMetadataUpdateJsonString);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandlePushUserMultimediaDeleteToUserEndpointsUpdate(InterserverMessageEventArgs e)
        {
            try
            {
                PushUserMultimediaDeleteToUserEndpoints request = e.Deserialize<PushUserMultimediaDeleteToUserEndpoints>();
                PushUserMultimediaDeleteToUserEndpoints_Here(request.UserIdSessionIdss, request.UserMultimediaMetadataUpdateJsonString);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}
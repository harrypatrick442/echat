using JSON;
using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Core;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Requests;
using MultimediaServerCore.Messages;
using Core.Messages.Responses;

namespace MultimediaServerCore
{
    public partial class MultimediaServerMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.MultimediaPrepareToUpload, HandleMultimediaPrepareToUpload},
                    { InterserverMessageTypes.MultimediaUploadUpdate , HandleMultimediaUploadUpdate },
                    {InterserverMessageTypes.MultimediaDelete, HandleMultimediaDelete}
                }
            );
        }

        private void HandleMultimediaPrepareToUpload(InterserverMessageEventArgs e)
        {
            PrepareToUploadRequest request = e.Deserialize<PrepareToUploadRequest>();
            PrepareToUploadResponse response;
            try
            {
                PrepareToUpload_Here(request.NodeIdRequestingUpload, request.MultimediaType, 
                    request.FileInfo, request.ScopeType, request.ScopingId, request.ScopingId2,
                    request.ScopingId3, out MultimediaFailedReason? failedReason,
                    out string multimediaToken);
                response = PrepareToUploadResponse.Success(multimediaToken, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = PrepareToUploadResponse.Failure(MultimediaFailedReason.ServerError, request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleMultimediaUploadUpdate(InterserverMessageEventArgs e)
        {
            try
            {
                MultimediaStatusUpdate request = e.Deserialize<MultimediaStatusUpdate>();
                MultimediaUploadUpdate_Here(request);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleMultimediaDelete(InterserverMessageEventArgs e)
        {
            DeleteMultimediaRequest request = e.Deserialize<DeleteMultimediaRequest>();
            SuccessTicketedResponse response;
            try
            {
                Delete_Here(request.RawMultimediaTokens);
                response = new SuccessTicketedResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}
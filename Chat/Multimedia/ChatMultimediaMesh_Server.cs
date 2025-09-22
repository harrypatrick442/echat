using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Core;
using MultimediaServerCore.Requests;
using MultimediaServerCore.Messages;
using Users.Messages.Client;
using MultimediaServerCore.Enums;
using MultimediaCore;
using JSON;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Chat.Messages.Client.Messages;

namespace MultimediaServerCore
{
    public partial class ChatMultimediaMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    { InterserverMessageTypes.ChatMultimediaUpload, HandleUpload},
                    { InterserverMessageTypes.ChatUpdatePendingUserMultimediaItemStatus, HandleUpdatePendingUserMultimediaItemStatus}
                }
            );
        }

        private void HandleUpload(InterserverMessageEventArgs e)
        {
            ChatMultimediaUploadRequest request = e.Deserialize<ChatMultimediaUploadRequest>();
            ChatMultimediaUploadResponse response;
            try
            {
                MultimediaFailedReason? failedReason = Upload_Here(request.MultimediaType, request.ScopeType, 
                    request.ConversationId, request.ConversationType, request.FileInfo, request.UserId, request.SessionId, request.XRating,
                    request.Description, out UserMultimediaItem userMultimediaItem, request.AlreadyCheckedPermission);
                response = new ChatMultimediaUploadResponse(userMultimediaItem, failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new ChatMultimediaUploadResponse(null, MultimediaFailedReason.ServerError, request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
            }
        }
        private void HandleUpdatePendingUserMultimediaItemStatus(InterserverMessageEventArgs e)
        {
            UpdatePendingUserMultimediaItemStatus message= e.Deserialize<UpdatePendingUserMultimediaItemStatus>();
            try
            {
                UpdatePendingUserMultimediaItemStatus_Here(message.StatusUpdate);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}
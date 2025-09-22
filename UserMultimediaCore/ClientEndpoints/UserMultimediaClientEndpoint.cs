using Core.Enums;
using Core.Messages;
using Logging;
using Core.Handlers;
using Core.InterserverComs;
using JSON;
using Core.Interfaces;
using MultimediaServerCore.Requests;
using MultimediaServerCore.Enums;
using UserMultimediaCore;
using MultimediaCore;

namespace Core.Authentication
{
    public class UserMultimediaClientEndpoint
    {
        private IClientEndpoint _Endpoint;
        private bool _Disposed = false;
        public UserMultimediaClientEndpoint(
            IClientEndpoint endpoint,
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler
        )
        {
            _Endpoint = endpoint;
            clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { global::MessageTypes.MessageTypes.MultimediaUploadProfilePicture, HandleUploadProfilePicture},
                { global::MessageTypes.MessageTypes.MultimediaUpdateProfilePictureMetadata, HandleUpdateUserProfilePictureMetadata },
                { global::MessageTypes.MessageTypes.MultimediaDeleteProfilePicture, HandleDeleteUserProfilePicture }
            });
        }
        protected void Initialize(IClientEndpoint endpoint)
        {
            _Endpoint = endpoint;
        }
        public void Dispose()
        {

        }
        private void HandleUploadProfilePicture(TypeTicketedAndWholePayload message)
        {
            UploadProfilePictureRequest request = Json
                .Deserialize<UploadProfilePictureRequest>(message.JsonString);
            try
            {
                MultimediaFailedReason? failedReason;
                if ((failedReason = UserMultimediaHandler.UploadProfilePicture(
                    request.FileInfo, _Endpoint.UserId, request.XRating,
                    request.VisibleTo, request.Description, request.SetAsMain, 
                    out UserMultimediaItem? userMultimediaItem)) == null)
                    _Endpoint.SendObject(UploadMultimediaResponse.Successful(userMultimediaItem!, request.Ticket));
                else
                    _Endpoint.SendObject(UploadMultimediaResponse.Failed(failedReason, request.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleUpdateUserProfilePictureMetadata(TypeTicketedAndWholePayload message)
        {
            UpdateProfilePictureMetadataRequest request = Json
                .Deserialize<UpdateProfilePictureMetadataRequest>(
                    message.JsonString);
            try
            {
                MultimediaFailedReason? failedReason = UserMultimediaHandler
                    .UpdateUserProfilePictureMetadata(
                        _Endpoint.UserId, request.VisibleTo, 
                        request.Description, request.MultimediaToken,
                        request.SetAsMain
                );
                _Endpoint.SendObject(new GenericMultimediaResponse(
                    failedReason, request.Ticket));
               
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleDeleteUserProfilePicture(TypeTicketedAndWholePayload message)
        {
            DeleteUserProfilePictureRequest request = Json
                .Deserialize<DeleteUserProfilePictureRequest>(
                    message.JsonString);
            try
            {
                MultimediaFailedReason? failedReason = UserMultimediaHandler
                    .DeleteUserProfilePicture(_Endpoint.UserId, request.MultimediaToken);
                 _Endpoint.SendObject(new GenericMultimediaResponse(
                     failedReason, request.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}

using MultimediaServerCore;
using Users.DAL;
using Core.Exceptions;
using Core.Events;
using MultimediaServerCore.Messages;
using JSON;
using MultimediaServerCore.Requests;
using MultimediaCore;
using Initialization.Exceptions;
namespace UserMultimediaCore
{
    public sealed class UserMultimediaEventListener
    {
        private static UserMultimediaEventListener _Instance;
        public static UserMultimediaEventListener Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(UserMultimediaEventListener));
            _Instance = new UserMultimediaEventListener();
            return _Instance;
        }
        public static UserMultimediaEventListener Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(UserMultimediaEventListener));
                return _Instance;
            }
        }
        private UserMultimediaEventListener()
        {
            MultimediaUploadsEventSource.MultimediaStatusChanged += HandleMultimediaStatusChanged;
        }
        private void HandleMultimediaStatusChanged(object sender, ItemEventArgs<MultimediaStatusUpdate> e) {
            MultimediaStatusUpdate multimediaUploadUpdate = e.Item;
            string? userMultimediaMetadataUpdateJsonString = null;
            switch (multimediaUploadUpdate.MultimediaType) {
                case MultimediaType.ProfilePicture:
                    DalUserProfiles.Instance.ModifyUserProfile(multimediaUploadUpdate.ScopingId, (userProfile) => {
                        UserMultimediaItem userMultimediaItem = userProfile.GetPicture(multimediaUploadUpdate.MultimediaToken);
                        if (userMultimediaItem == null)
                            return userProfile;
                        userMultimediaItem.Status = multimediaUploadUpdate.Status;
                        userMultimediaMetadataUpdateJsonString = Json.Serialize(
                            new UserMultimediaMetadataUpdate(multimediaUploadUpdate.ScopingId, MultimediaType.ProfilePicture, userMultimediaItem));
                        return userProfile;
                    });
                    UserMultimediaMesh.Instance.PushUserMultimediaMetadataUpdateToUserEndpoints(multimediaUploadUpdate.ScopingId,
                        userMultimediaMetadataUpdateJsonString!);
                    break;
            }
        }
    }
}
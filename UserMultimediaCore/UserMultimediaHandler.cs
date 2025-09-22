using Core.Timing;
using MultimediaServerCore;
using MultimediaServerCore.Enums;
using UsersEnums;
using FileInfo = Core.Messages.Messages.FileInfo;
using Users.DAL;
using Logging;
using MultimediaServerCore.Requests;
using JSON;
using Users;
using MultimediaCore;
namespace UserMultimediaCore
{
    public static class UserMultimediaHandler
    {
        public static MultimediaFailedReason? UploadProfilePicture(
            FileInfo fileInfo, long userId, XRating xRating,
            VisibleTo visibleTo, string description, bool setAsMain, out UserMultimediaItem? userMultimediaItem)
        {
            MultimediaFailedReason? failedReason = MultimediaServerMesh.Instance.PrepareToUpload(
                MultimediaType.ProfilePicture, fileInfo, MultimediaScopeType.UserProfile, userId,
                null, null, out string? multimediaToken);
            userMultimediaItem = null;
            if (failedReason != null)
                return failedReason;
            try
            {
                UserMultimediaItem? userMultimediaItemInternal = UserMultimediaItem.NewPending(
                    multimediaToken!, xRating, visibleTo, description, setAsMain);
                DalUserProfiles.Instance.ModifyUserProfile(userId, (userProfile) =>
                {
                    if (userProfile != null)
                        userProfile.AddPicture(userMultimediaItemInternal);
                    return userProfile;
                });
                userMultimediaItem = userMultimediaItemInternal;
                return null;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return MultimediaFailedReason.ServerError;
            }
        }
        public static MultimediaFailedReason? UpdateUserProfilePictureMetadata(
            long userId, VisibleTo visibleTo, string description, 
            string multimediaToken, bool setAsMain)
        {
            try
            {
                string? userMultimediaMetadataUpdateJsonString = null;
                DalUserProfiles.Instance.ModifyUserProfile(userId, (userProfile) =>
                {
                    if (userProfile == null)
                        throw new NullReferenceException(nameof(userProfile));
                    UserMultimediaItem picture = userProfile.GetPicture(multimediaToken);
                    if (picture == null)
                        return userProfile;
                    picture.Description = UserMultimediaConstrainer.Description(description);
                    picture.VisibleTo = visibleTo;
                    picture.SetAsMain = setAsMain 
                    ? (picture.SetAsMain!=null? picture.SetAsMain:(long?)TimeHelper.MillisecondsNow)
                    : null;
                    userMultimediaMetadataUpdateJsonString = 
                        Json.Serialize(new UserMultimediaMetadataUpdate(userId, MultimediaType.ProfilePicture, picture));
                    return userProfile;
                });
                if (userMultimediaMetadataUpdateJsonString != null)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            UserMultimediaMesh.Instance.PushUserMultimediaMetadataUpdateToUserEndpoints(
                                userId, userMultimediaMetadataUpdateJsonString);
                        }
                        catch(Exception ex)
                        {
                            Logs.Default.Error(ex);
                        }
                    }).Start();
                }
                return null;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return MultimediaFailedReason.ServerError;
            }

        }
        public static MultimediaFailedReason? DeleteUserProfilePicture(
            long userId, string multimediaToken)
        {
            try
            {
                long uploadedAt = TimeHelper.MillisecondsNow;
                bool deleted = false;
                DalUserProfiles.Instance.ModifyUserProfile(userId, (userProfile) =>
                {
                    if (userProfile != null)
                    {
                        /*Safer to do before. Though it may not exist any more. It will end up simply
                        not finding the file and ticking it off as deleted. 
                        No risk of another file being given same multimediaToken.
                        It's time based and doesnt go backwards.*/
                        MultimediaServerMesh.Instance.Delete(multimediaToken);
                        deleted = userProfile.RemovePicture(multimediaToken); 
                    }
                    return userProfile;
                });
                if (deleted)
                    UserMultimediaMesh.Instance.PushUserMultimediaDelete(new MultimediaDelete(userId, multimediaToken));
                return null;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return MultimediaFailedReason.ServerError;
            }
        }
    }
}
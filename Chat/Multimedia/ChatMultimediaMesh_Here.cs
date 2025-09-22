using Chat;
using Core.Chat;
using Core.DAL;
using EChatEndpoints.WebsocketServers;
using Logging;
using MultimediaCore;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Messages;
using UserRouting;
using UsersEnums;
using FileInfo = Core.Messages.Messages.FileInfo;

namespace MultimediaServerCore
{
    public partial class ChatMultimediaMesh
    {//This is where requestingNodeId comes from. Its the machine associated with the conversation
        //Not the machine associated with the user.

        public MultimediaFailedReason? Upload_Here(
            MultimediaType multimediaType, MultimediaScopeType scopeType,
            long conversationId, ConversationType conversationType, FileInfo fileInfo, long userId,
            long? sessionId, XRating xRating, string description, out UserMultimediaItem? userMultimediaItem,
            bool alreadyCheckedPermission)
        {
            if (!alreadyCheckedPermission&&!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(conversationId, conversationType,
                userId, out IConversation useUserIds, out ChatFailedReason failedReason))
            {
                userMultimediaItem = null;
                return MultimediaFailedReason.Permissions;
            }
            switch (multimediaType)
            {
                case MultimediaType.ConversationPicture:
                    return UploadRoomPicture_Here(conversationId, fileInfo, userId, xRating, description, out userMultimediaItem);
                case MultimediaType.MessagePicture:
                    return UploadMessagePicture_Here(scopeType, conversationId, fileInfo, userId, sessionId, xRating, description, out userMultimediaItem);
                case MultimediaType.MessageVideo:
                    return UploadMessageVideo_Here(scopeType, conversationId, fileInfo, userId, sessionId, xRating, description, out userMultimediaItem);
                default:
                    throw new NotImplementedException();
            }

        }
        private MultimediaFailedReason? UploadRoomPicture_Here(
            long conversationId, FileInfo fileInfo, long userId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            MultimediaFailedReason? failedReason = null;
            UserMultimediaItem? userMultimediaItemInternal = null;
            try
            {
                bool chatRoomInfoChanged = false;
                DalChatRoomInfos.Instance.ModifyWithinLock(conversationId, (chatRoomInfo) =>
                {
                    if (!chatRoomInfo.HasPermission(userId, AdministratorPrivilages.ChangeRoomPicture))
                    {
                        failedReason = MultimediaFailedReason.Permissions;
                        return chatRoomInfo;
                    }
                    failedReason = MultimediaServerMesh.Instance.PrepareToUpload(
                        MultimediaType.ConversationPicture, fileInfo, MultimediaScopeType.ChatRoom,
                        conversationId, userId, null, out string? multimediaToken);
                    if (failedReason != null)
                    {
                        return chatRoomInfo;
                    }
                    userMultimediaItemInternal = UserMultimediaItem.NewPending(
                        multimediaToken!, xRating, VisibleTo.Public, description, false);
                    chatRoomInfo.SetPendingMainPicture(userMultimediaItemInternal);
                    chatRoomInfoChanged = true;
                    return chatRoomInfo;
                });
                if (chatRoomInfoChanged)
                {
                    ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
                    chatRoom?.InfoChanged(false);
                }
                userMultimediaItem = userMultimediaItemInternal;
                return failedReason;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                userMultimediaItem = null;
                return MultimediaFailedReason.ServerError;
            }
        }
        private MultimediaFailedReason? UploadMessagePicture_Here(
            MultimediaScopeType scopeType,
            long conversationId, FileInfo fileInfo, long userId, long? sessionId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            try
            {
                MultimediaFailedReason? failedReason = MultimediaServerMesh.Instance
                    .PrepareToUpload(
                    MultimediaType.MessagePicture,  fileInfo, scopeType,
                    conversationId, userId, sessionId, out string? multimediaToken);
                if (failedReason != null)
                {
                    userMultimediaItem = null;
                    return failedReason;
                }
                userMultimediaItem = UserMultimediaItem.NewPending(
                    multimediaToken!, xRating, VisibleTo.Public, description, false);
                return failedReason;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                userMultimediaItem = null;
                return MultimediaFailedReason.ServerError;
            }
        }
        private MultimediaFailedReason? UploadMessageVideo_Here(
            MultimediaScopeType scopeType,
            long conversationId, FileInfo fileInfo, long userId, long? scopingId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            try
            {
                MultimediaFailedReason? failedReason = MultimediaServerMesh.Instance.PrepareToUpload(
                    MultimediaType.MessageVideo, fileInfo, scopeType,
                    conversationId, userId, scopingId, out string? multimediaToken);
                if (failedReason != null)
                {
                    userMultimediaItem = null;
                    return failedReason;
                }
                userMultimediaItem = UserMultimediaItem.NewPending(
                    multimediaToken!, xRating, VisibleTo.Public, description, false);
                return failedReason;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                userMultimediaItem = null;
                return MultimediaFailedReason.ServerError;
            }
        }
        private void UpdatePendingUserMultimediaItemStatus_Here(MultimediaStatusUpdate statusUpdate) {

            IUserChatClientEndpoint clientEndpoint = (IUserChatClientEndpoint)CoreUserRoutingTable
                .Instance.GetLocalEndpoint((long)statusUpdate.ScopingId2, (long)statusUpdate.ScopingId3);
            if (clientEndpoint == null) return;
            clientEndpoint.ChatClientEndpoint.UpdateMultimediaItemStatus(statusUpdate.MultimediaToken, statusUpdate.Status);
            clientEndpoint.SendObject(statusUpdate);
        }
    }
}
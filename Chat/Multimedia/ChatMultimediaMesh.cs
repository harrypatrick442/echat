using Core.Exceptions;
using Shutdown;
using InterserverComs;
using Chat;
using MultimediaCore;
using UsersEnums;
using FileInfo = Core.Messages.Messages.FileInfo;
using MultimediaServerCore.Enums;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using MultimediaServerCore.Messages;
using Chat.Messages.Client.Messages;
using UserRouting;

namespace MultimediaServerCore
{
    public partial class ChatMultimediaMesh
    {
        private static ChatMultimediaMesh? _Instance;
        public static ChatMultimediaMesh Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(ChatMultimediaMesh));
            _Instance = new ChatMultimediaMesh();
            return _Instance;
        }
        public static ChatMultimediaMesh Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(ChatMultimediaMesh));
                return _Instance;
            }
        }
        private long _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private ChatMultimediaMesh()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.MultimediaServerMesh);

        }
        #region Methods
        #region Public
        public MultimediaFailedReason? UploadRoomPicture(
            long conversationId, ConversationType conversationType, FileInfo fileInfo, long userId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            return Upload(MultimediaType.ConversationPicture, MultimediaScopeType.ChatRoom, 
                conversationId, conversationType, fileInfo, userId, null, xRating, description, 
                out userMultimediaItem, true);
        }
        public MultimediaFailedReason? UploadChatRoomMessagePicture(
            long conversationId, ConversationType conversationType, FileInfo fileInfo, long userId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            return Upload(MultimediaType.MessagePicture, MultimediaScopeType.ChatRoom, conversationId,
                conversationType, fileInfo, userId, null, xRating, description, out userMultimediaItem,
                true);
        }
        public MultimediaFailedReason? UploadChatRoomMessageVideo(
            long conversationId, ConversationType conversationType, FileInfo fileInfo, long userId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            return Upload(MultimediaType.MessageVideo, MultimediaScopeType.ChatRoom, conversationId,
                conversationType ,fileInfo, userId, null, xRating, description, out userMultimediaItem,
                true);
        }
        public MultimediaFailedReason? UploadPmMessagePicture(
            long conversationId, ConversationType conversationType, FileInfo fileInfo, long userId, long sessionId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            return Upload(MultimediaType.MessagePicture, MultimediaScopeType.Pm, conversationId,
                conversationType, fileInfo, userId, sessionId, xRating, description, out userMultimediaItem,
                false);
        }
        public MultimediaFailedReason? UploadPmMessageVideo(
            long conversationId, ConversationType conversationType, FileInfo fileInfo, long userId, long sessionId, XRating xRating,
            string description, out UserMultimediaItem? userMultimediaItem)
        {
            return Upload(MultimediaType.MessageVideo, MultimediaScopeType.Pm, conversationId,
                conversationType, fileInfo, userId, sessionId, xRating, description, out userMultimediaItem,
                false);
        }
        public void UpdatePendingUserMultimediaItemStatus(
            MultimediaStatusUpdate statusUpdate)
        {

            int? nodeId = CoreUserRoutingTable.Instance.GetNodeIdForUserIdSessionId((long)statusUpdate.ScopingId2, (long)statusUpdate.ScopingId3);
            if (nodeId == null) {
                return;
            }
            OperationRedirectHelper.OperationRedirectedToNode<UpdatePendingUserMultimediaItemStatus>(
                (int)nodeId,
                () =>
                {
                    UpdatePendingUserMultimediaItemStatus_Here(statusUpdate);
                },
                () => new UpdatePendingUserMultimediaItemStatus(statusUpdate)
            );
        }
        #endregion Public
        #region Private
        public MultimediaFailedReason? Upload(
            MultimediaType multimediaType, MultimediaScopeType scopeType,
            long conversationId, ConversationType conversationType, FileInfo fileInfo, long userId, 
            long? sessionId, XRating xRating, string description, out UserMultimediaItem? userMultimediaItem, 
            bool alreadyCheckedPermission)
        {
            UserMultimediaItem? userMultimediaItemInternal = null;
            MultimediaFailedReason? failedReason = null;
            OperationRedirectHelper.OperationRedirectedToNode<ChatMultimediaUploadRequest, ChatMultimediaUploadResponse>(
            ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                () =>
                {
                    failedReason = Upload_Here(multimediaType, scopeType, conversationId, conversationType, fileInfo,
                        userId, sessionId, xRating, description, out userMultimediaItemInternal, alreadyCheckedPermission);
                },
                () => new ChatMultimediaUploadRequest(multimediaType, scopeType, conversationId,
                conversationType, fileInfo, userId, sessionId, xRating, description, alreadyCheckedPermission),
                (response) =>
                {
                    failedReason = response.FailedReason;
                    userMultimediaItemInternal = response.UserMultimediaItem;
                },
                _CancellationTokenSourceDisposed.Token
            );
            userMultimediaItem = userMultimediaItemInternal;
            return failedReason;
        }
        private void Dispose()
        {
            _CancellationTokenSourceDisposed.Cancel();
        }
        #endregion
        #endregion Methods
    }
}
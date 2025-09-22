using MultimediaServerCore;
using Core.Exceptions;
using Core.Events;
using MultimediaServerCore.Messages;
using JSON;
using MultimediaCore;
using Chat;
using Core.DAL;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Exceptions;
using Core.Interfaces;
using UserRouting;
using Chat.Endpoints;
namespace UserMultimediaCore
{
    public sealed class ChatMultimediaEventListener
    {
        private static ChatMultimediaEventListener _Instance;
        public static ChatMultimediaEventListener Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(ChatMultimediaEventListener));
            _Instance = new ChatMultimediaEventListener();
            return _Instance;
        }
        public static ChatMultimediaEventListener Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(ChatMultimediaEventListener));
                return _Instance;
            }
        }
        private ChatMultimediaEventListener()
        {
            MultimediaUploadsEventSource.MultimediaStatusChanged += HandleMultimediaStatusChanged;
        }
        private void HandleMultimediaStatusChanged(object sender, ItemEventArgs<MultimediaStatusUpdate> e) {
            MultimediaStatusUpdate statusUpdate = e.Item;
            //string? userMultimediaMetadataUpdateJsonString = null;

            switch (statusUpdate.ScopeType)
            {
                case MultimediaScopeType.ChatRoom:
                    switch (statusUpdate.MultimediaType)
                    {
                        case MultimediaType.ConversationPicture:
                            HandleChatRoomPictureStatusChange(statusUpdate);
                            break;
                        case MultimediaType.MessagePicture:
                            HandleChatRoomMessageItemStatusChange(statusUpdate);
                            break;
                        case MultimediaType.MessageVideo:
                            HandleChatRoomMessageItemStatusChange(statusUpdate);
                            break;
                    }
                    return;

                case MultimediaScopeType.Pm:
                    switch (statusUpdate.MultimediaType)
                    {
                        case MultimediaType.MessagePicture:
                            HandlePmMessageItemStatusChange(statusUpdate);
                            break;
                        case MultimediaType.MessageVideo:
                            HandlePmMessageItemStatusChange(statusUpdate);
                            break;
                    }
                    return;
            }
        }
        private void HandleChatRoomPictureStatusChange(MultimediaStatusUpdate statusUpdate) {

            long conversationId = statusUpdate.ScopingId;
            bool live = statusUpdate.Status.Equals(MultimediaItemStatus.Live);
            string oldMainPicture = null;
            ChatRoomInfo chatRoomInfoOut = null;
            DalChatRoomInfos.Instance.ModifyWithinLock(conversationId,
                (chatRoomInfo) =>
                {
                    UserMultimediaItem pendingMainPicture = chatRoomInfo.PendingMainPicture;
                    if (pendingMainPicture == null)
                    {
                        throw new MultimediaException(MultimediaFailedReason.ServerError, $"{nameof(pendingMainPicture)} was null");
                    }
                    if (pendingMainPicture.MultimediaToken != statusUpdate.MultimediaToken)
                    {
                        throw new MultimediaException(MultimediaFailedReason.ServerError, $"{nameof(pendingMainPicture.MultimediaToken)} did not match {nameof(statusUpdate.MultimediaToken)}");
                    }
                    pendingMainPicture.Status = statusUpdate.Status;
                    if (live)
                    {
                        oldMainPicture = chatRoomInfo.MainPicture;
                        if (oldMainPicture != null)
                        {
                            MultimediaServerMesh.Instance.Delete(oldMainPicture);
                        }
                        chatRoomInfo.SetMainPicture(chatRoomInfo.PendingMainPicture.MultimediaToken);
                    }
                    chatRoomInfoOut = chatRoomInfo;
                    return chatRoomInfo;
                }
            );
            if (!live) return;
            ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
            if (chatRoom == null) return;
            chatRoom.InfoChanged(true);
        }

        private void HandleChatRoomMessageItemStatusChange(MultimediaStatusUpdate statusUpdate)
        {
            //TODO ideally only needs to go to a specific sessoin too. The one with the user uploading the image. This is happening before the message is sent.They are still in editor.
            //TODO may not need this...

            ChatRoom chatRoom = ChatRooms.Instance.GetIfExists((long)statusUpdate.ScopingId);
            if (chatRoom == null) return;
            long userId = (long)statusUpdate.ScopingId2;
            ChatRoomClientEndpoint[] chatRoomClientEndpoints = chatRoom.GetClientEndpoints(userId);
            if (chatRoomClientEndpoints == null) return;
            foreach (ChatRoomClientEndpoint chatRoomClientEndpoint in chatRoomClientEndpoints)
            {
                chatRoomClientEndpoint.UpdateMultimediaItemStatus(statusUpdate.MultimediaToken, statusUpdate.Status);
            }
            IClientEndpoint[] clientEndpoints = CoreUserRoutingTable
                .Instance.GetLocalEndpointsForUser((long)statusUpdate.ScopingId2);
            string statusUpdateSerialized = Json.Serialize(statusUpdate);
            foreach (IClientEndpoint clientEndpoint in clientEndpoints) {
                clientEndpoint.SendJSONString(statusUpdateSerialized);
            }
            /*
            UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                statusUpdate, new long[] { userId });
            *///May like to make this session specific but cost here is smaller than for pms.
        }   
        private void HandlePmMessageItemStatusChange(MultimediaStatusUpdate statusUpdate)
        {
            ChatMultimediaMesh.Instance.UpdatePendingUserMultimediaItemStatus(statusUpdate);
        }
    }
}
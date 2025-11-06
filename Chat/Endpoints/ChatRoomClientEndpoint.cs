using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using JSON;
using Logging;
using Core;
using Core.Interfaces;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Chat.Messages.Client.Messages;
using MultimediaCore;
using MultimediaServerCore.Enums;
using MultimediaServerCore;
using MultimediaServerCore.Requests;
using Users.Messages.Client;
using Users;
using MultimediaServerCore.Messages.Messages;

namespace Chat.Endpoints
{
    public class ChatRoomClientEndpoint
    {
        public event EventHandler OnDispose;
        private readonly object _LockObjectDisposed = new object();
        private bool _Disposed = false;
        public bool Disposed
        {
            get
            {
                lock (_LockObjectDisposed)
                {
                    return _Disposed;
                }
            }
        }
        public long UserId { get; }
        private IClientEndpointLight _ClientEndpoint;
        private ChatRoom _ChatRoom;
        private Action _RemoveClientMessageTypeMappings;
        private PendingMultimediaItems _PendingMultimediaItems = new PendingMultimediaItems();
        public ChatRoomClientEndpoint(
            long userId,
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            ChatRoom clientEndpoints,
            IClientEndpointLight chatRoomSocket)
        {
            UserId = userId;
            _ChatRoom = clientEndpoints;
            _ClientEndpoint = chatRoomSocket;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler
                .AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                    { MessageTypes.ChatRoomSendMessage, HandleMessage},
                    { MessageTypes.ChatLoadMessagesHistory, LoadMessagesHistory },
                    { MessageTypes.ChatUnreactToMessage, UnreactToMessage},
                    { MessageTypes.ChatReactToMessage, ReactToMessage},
                    { MessageTypes.ChatModifyMessage, ModifyMessage},
                    { MessageTypes.ChatDeleteMessages, DeleteMessages},
                    { MessageTypes.ChatUpdateRoomInfo, UpdateRoomInfo},
                    { MessageTypes.ChatUploadRoomPicture, UploadChatRoomPicture},
                    { MessageTypes.ChatUploadMessagePicture, HandleUploadMessagePicture},
                    { MessageTypes.ChatUploadMessageVideo, HandleUploadMessageVideo},
                    { MultimediaServerCore.MessageTypes.MultimediaDeletePending, HandleDeletePendingMultimediaItem},
                    { MessageTypes.ChatGetAdministrators, HandleGetAdministrators},
                    { MessageTypes.ChatSetAdministrator, HandleSetAdministrator},
                    { MessageTypes.ChatRemoveAdministrator, HandleRemoveAdministrator},
                    { Users.MessageTypes.UsernameSearchSearch , HandleUsernameSearchSearch},
                    { MessageTypes.ChatBanUser, HandleBanUser },
                    { MessageTypes.ChatUnbanUser, HandleUnbanUser }
                });
        }
        public void SendJSONString(string jsonString)
        {
            _ClientEndpoint.SendJSONString(jsonString);
        }
        private void HandleMessage(TypeTicketedAndWholePayload message)
        {
            SendChatRoomMessageRequest request = Json.Deserialize<SendChatRoomMessageRequest>(message.JsonString);
            request.ChatRoomMessage.UserId = UserId;
            request.ChatRoomMessage.UserMultimediaItems = _PendingMultimediaItems
                .GetPendingUserMultimediaItems(request.ChatRoomMessage.UserMultimediaItems);
            _ChatRoom.HandleChatRoomMessage(request.ChatRoomMessage);
            SendChatRoomMessageResponse response = SendChatRoomMessageResponse.Successful(request.Ticket);
            SendJSONString(Json.Serialize(response));
        }
        private void LoadMessagesHistory(TypeTicketedAndWholePayload message)
        {
            LoadMessagesHistoryRequest request = Json.Deserialize<LoadMessagesHistoryRequest>(message.JsonString);
            _ChatRoom.HandleLoadMessageHistory(request, _ClientEndpoint);
        }
        private void ReactToMessage(TypeTicketedAndWholePayload message)
        {
            ReactToMessage request = Json.Deserialize<ReactToMessage>(message.JsonString);
            request.MessageReaction.UserId = UserId;
            _ChatRoom.HandleReactToMessage(request);
        }
        private void UnreactToMessage(TypeTicketedAndWholePayload message)
        {
            UnreactToMessage request = Json.Deserialize<UnreactToMessage>(message.JsonString);
            request.MessageReaction.UserId = UserId;
            _ChatRoom.HandleUnreactToMessage(request);
        }

        private void ModifyMessage(TypeTicketedAndWholePayload message)
        {
            ModifyMessage request = Json.Deserialize<ModifyMessage>(message.JsonString);
            if (request.Message == null) return;
            request.Message.UserId = UserId;
            _ChatRoom.HandleModifyMessage(request, _ClientEndpoint);
        }

        private void DeleteMessages(TypeTicketedAndWholePayload message)
        {
            DeleteMessagesRequest request = Json.Deserialize<DeleteMessagesRequest>(message.JsonString);
            _ChatRoom.HandleDeleteMessage(request, _ClientEndpoint, UserId);
        }
        private void UpdateRoomInfo(TypeTicketedAndWholePayload message)
        {
            UpdateChatRoomInfoRequest request = Json.Deserialize<UpdateChatRoomInfoRequest>(message.JsonString);
            request.UserId = UserId;
            _ChatRoom.HandleUpdateRoomInfo(request);
            _ClientEndpoint.SendObject(new UpdateChatRoomInfoResponse(request.Ticket));
        }
        private void UploadChatRoomPicture(TypeTicketedAndWholePayload message)
        {
            UploadChatRoomPictureRequest request = Json
                .Deserialize<UploadChatRoomPictureRequest>(message.JsonString);
            MultimediaFailedReason? failedReason = _ChatRoom.HandleUploadChatRoomPicture(request, UserId, out UserMultimediaItem userMultimediaItem);
            if (failedReason == null)
                _ClientEndpoint.SendObject(UploadMultimediaResponse.Successful(userMultimediaItem!, request.Ticket));
            else
                _ClientEndpoint.SendObject(UploadMultimediaResponse.Failed(failedReason, request.Ticket));
        }
        private void HandleUploadMessagePicture(TypeTicketedAndWholePayload message)
        {
            UploadMessagePictureRequest request = Json
                .Deserialize<UploadMessagePictureRequest>(message.JsonString);
            MultimediaFailedReason? failedReason = _ChatRoom.HandleUploadMessagePicture(request, UserId, out UserMultimediaItem userMultimediaItem);
            if (failedReason != null)
            {
                _ClientEndpoint.SendObject(UploadMultimediaResponse.Failed(failedReason, request.Ticket));
                return;
            }
            _PendingMultimediaItems.AddPendingMultimediaItemAndOverflow(userMultimediaItem);
            _ClientEndpoint.SendObject(UploadMultimediaResponse.Successful(userMultimediaItem!, request.Ticket));
        }
        private void HandleUploadMessageVideo(TypeTicketedAndWholePayload message)
        {
            UploadMessageVideoRequest request = Json
                .Deserialize<UploadMessageVideoRequest>(message.JsonString);
            MultimediaFailedReason? failedReason = _ChatRoom.HandleUploadMessageVideo(request, UserId, out UserMultimediaItem userMultimediaItem);
            if (failedReason != null)
            {
                _ClientEndpoint.SendObject(UploadMultimediaResponse.Failed(failedReason, request.Ticket));
                return;
            }
            _PendingMultimediaItems.AddPendingMultimediaItemAndOverflow(userMultimediaItem);
            _ClientEndpoint.SendObject(UploadMultimediaResponse.Successful(userMultimediaItem!, request.Ticket));
        }
        public void HandleDeletePendingMultimediaItem(TypeTicketedAndWholePayload message)
        {
            DeletePendingUserMultimediaItem request = Json
                .Deserialize<DeletePendingUserMultimediaItem>(message.JsonString);
            _PendingMultimediaItems.Delete(request.MultimediaToken);

        }
        private void HandleGetAdministrators(TypeTicketedAndWholePayload message)
        {
            GetAdministratorsRequest request = Json.Deserialize<GetAdministratorsRequest>(message.JsonString);
            GetAdministratorsResponse response;
            try
            {
                Administrator[] administrators = _ChatRoom.GetAdministrators(UserId, out AdministratorsFailedReason? failedReason);
                response = new GetAdministratorsResponse(administrators, failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                response = new GetAdministratorsResponse(null, AdministratorsFailedReason.ServerError, request.Ticket);
                Logs.Default.Error(ex);
            }
            _ClientEndpoint.SendObject(response);
        }
        private void HandleSetAdministrator(TypeTicketedAndWholePayload message)
        {
            SetAdministratorRequest request = Json.Deserialize<SetAdministratorRequest>(message.JsonString);
            SetAdministratorResponse response;
            try
            {
                AdministratorsFailedReason? failedReason = _ChatRoom.SetAdministrator(UserId, request.UserId, request.Privilages);
                response = new SetAdministratorResponse(failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                response = new SetAdministratorResponse(AdministratorsFailedReason.ServerError, request.Ticket);
                Logs.Default.Error(ex);
            }
            _ClientEndpoint.SendObject(response);
        }
        private void HandleRemoveAdministrator(TypeTicketedAndWholePayload message)
        {
            RemoveAdministratorRequest request = Json.Deserialize<RemoveAdministratorRequest>(message.JsonString);
            RemoveAdministratorResponse response;
            try
            {
                AdministratorsFailedReason? failedReason = _ChatRoom.RemoveAdministrator(UserId, request.UserId, request.AllowRemoveOnlyFullAdmin);
                response = new RemoveAdministratorResponse(failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                response = new RemoveAdministratorResponse(AdministratorsFailedReason.ServerError, request.Ticket);
                Logs.Default.Error(ex);
            }
            _ClientEndpoint.SendObject(response);
        }
        private void HandleBanUser(TypeTicketedAndWholePayload message)
        {
            BanUserRequest request = Json.Deserialize<BanUserRequest>(message.JsonString);
            BanUserResponse response;
            try
            {
                AdministratorsFailedReason? failedReason = _ChatRoom.BanUser(
                    UserId, request.UserId);
                response = new BanUserResponse(failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                response = new BanUserResponse(AdministratorsFailedReason.ServerError, request.Ticket);
                Logs.Default.Error(ex);
            }
            _ClientEndpoint.SendObject(response);
        }
        private void HandleUnbanUser(TypeTicketedAndWholePayload message)
        {
            UnbanUserRequest request = Json.Deserialize<UnbanUserRequest>(message.JsonString);
            UnbanUserResponse response;
            try
            {
                AdministratorsFailedReason? failedReason = _ChatRoom.BanUser(
                    UserId, request.UserId);
                response = new UnbanUserResponse(failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                response = new UnbanUserResponse(AdministratorsFailedReason.ServerError, request.Ticket);
                Logs.Default.Error(ex);
            }
            _ClientEndpoint.SendObject(response);
        }
        private void HandleUsernameSearchSearch(TypeTicketedAndWholePayload message)
        {
            UsernameSearchSearchRequest request = Json
                .Deserialize<UsernameSearchSearchRequest>(message.JsonString);
            try
            {
                bool success = UsersMesh.Instance.UsernameSearchSearch(request.Str, Configurations.Lengths.USERNAME_SEARCH_MAX_N_ENTRIES_RESULT, out long[] userIds);
                if (userIds != null)
                {
                    _ChatRoom.Info.UsingJoinedUsers((joinedUsers) =>
                    {
                        if (joinedUsers == null)
                        {
                            userIds = null;
                        }
                        else
                        {
                            userIds = userIds.Where(i => joinedUsers.Contains(i)).ToArray();
                        }
                    });
                }
                if (!success)
                {
                    _ClientEndpoint.SendObject(new UsernameSearchSearchResponse(null, false, request.Ticket));
                    return;
                }
                _ClientEndpoint.SendObject(new UsernameSearchSearchResponse(userIds, true, request.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                _ClientEndpoint.SendObject(new UsernameSearchSearchResponse(null, false, request.Ticket));
            }
        }
        public void UpdateMultimediaItemStatus(string multimediaToken, MultimediaItemStatus status)
        {
            _PendingMultimediaItems.UpdateMultimediaItemStatus(multimediaToken, status);
        }
        ~ChatRoomClientEndpoint()
        {
            Dispose();
        }
        public void Dispose()
        {
            EventHandler onDispose;
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                _Disposed = true;
                onDispose = OnDispose;
                try
                {
                    _PendingMultimediaItems.Dispose();
                }
                catch (Exception ex) { Logs.Default.Error(ex); }
            }
            _RemoveClientMessageTypeMappings();
            onDispose?.Invoke(this, EventArgs.Empty);
        }
    }
}
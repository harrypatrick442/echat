using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using Core.Messages.Responses;
using JSON;
using Logging;
using Core.Interfaces;
using Core;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using MultimediaCore;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Requests;
using MultimediaServerCore;
using Core.DAL;
using Chat.Interfaces;
using MultimediaServerCore.Messages.Messages;

namespace Chat.Endpoints
{
    public class ChatClientEndpoint
    {
        private IClientEndpoint _ClientEndpoint;
        private long _MyUserId { get { return _ClientEndpoint.UserId; } }
        private Action _RemoveClientMessageTypeMappings;
        private PendingMultimediaItems _PendingMultimediaItems = new PendingMultimediaItems();
        private ActiveConversations _ActiveConversations = new ActiveConversations();
        public ChatClientEndpoint(
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpoint clientEndpoint)
        {
            _ClientEndpoint = clientEndpoint;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { MessageTypes.ChatGetConversationSnapshots, GetMyConversationSnapshots },
                { MessageTypes.ChatGetPmConversationWithLatestMessages, GetPmConversation},
                //{ ClientMessageTypes.ChatGetConversation, GetConversation},
                { MessageTypes.ClientMessage, SendMessage},
                { MessageTypes.ChatFetchIndividualMessages, FetchIndividualMessages},
                { MessageTypes.ChatLoadMessagesHistory, LoadMessagesHistory},
                { MessageTypes.ChatUnreactToMessage, UnreactToMessage},
                { MessageTypes.ChatReactToMessage, ReactToMessage},
                { MessageTypes.ChatModifyMessage, ModifyMessage},
                { MessageTypes.ChatDeleteMessages, DeleteMessages},
                { MessageTypes.ChatGetUserRooms, GetUserRooms},
                { MessageTypes.ChatModifyUserRooms, ModifyUserRooms},
                { MessageTypes.ChatGetRoomSummarys, GetRoomSummarys},
                { MessageTypes.ChatCreateRoom, CreateRoom},
                { MessageTypes.ChatPmUploadMessagePicture, HandlePmUploadMessagePicture},
                { MessageTypes.ChatPmUploadMessageVideo, HandlePmUploadMessageVideo},
                { MultimediaServerCore.MessageTypes.MultimediaDeletePending, HandleDeletePendingMultimediaItem},
                { MessageTypes.ChatGetWallConversation, GetWallConversation},
                { MessageTypes.ChatGetWallCommentsConversation, GetWallCommentsConversation},
                { MessageTypes.ChatRemoveUserFromActiveConversations, RemoveUserFromActiveConversations},
                { MessageTypes.ChatRoomInvite, HandleRoomInvite},
                { MessageTypes.ChatGetMyReceivedInvites, HandleGetMyReceivedInvites},
                { MessageTypes.ChatGetMySentInvites, HandleGetMySentInvites },
                { MessageTypes.ChatAcceptRoomInvite, HandleAcceptRoomInvite },
                { MessageTypes.ChatRejectRoomInvite, HandleRejectRoomInvite},
                { MessageTypes.ChatCancelRoomInvite, HandleCancelRoomInvite },
                { MessageTypes.ChatLeaveRoom, HandleLeaveRoom },
                { MessageTypes.ChatSearchRooms, HandleSearchRooms },
                { MessageTypes.ChatSetSeenMessage, HandleSetSeenMessage }
            });
        }
        private void GetMyConversationSnapshots(TypeTicketedAndWholePayload message)
        {
            GetConversationSnapshotsRequest request = Json.Deserialize<GetConversationSnapshotsRequest>(message.JsonString);
            GetConversationSnapshotsResponse response;
            if (
                ChatManager.Instance.GetConversationSnapshots(
                    _MyUserId,
                    request.IdFromInclusive,
                    request.IdToExclusive,
                    request.NEntries,
                    out ConversationSnapshot[] messages,
                    out MessageReaction[] messageReactions,
                    out MessageUserMultimediaItem[] messageUserMultimediaItems,
                    out ChatFailedReason failedReason
                )
             )
                response = GetConversationSnapshotsResponse.Success(
                    messages, messageReactions, messageUserMultimediaItems, failedReason, request.Ticket);
            else
                response = GetConversationSnapshotsResponse.Failed(failedReason, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void FetchIndividualMessages(TypeTicketedAndWholePayload message)
        {
            FetchIndividualMessagesRequest request = Json.Deserialize<FetchIndividualMessagesRequest>(message.JsonString);
            FetchConversationIndividualMessagesResult[] results = ChatManager.Instance.FetchIndividualMessages(
                    _MyUserId,
                    request.ConversationAndMessageIds
                );
            FetchIndividualMessagesResponse response = new FetchIndividualMessagesResponse(
                results, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        /*
        private void GetConversation(TypeTicketedAndWholePayload message)
        {
            GetConversationRequest getConversationRequest = Json.Deserialize<GetConversationRequest>(message.JsonString);
            GetConversationResponse response;
            if (
               ChatManager.Instance.GetConversation(
                   getConversationRequest.ConversationId,
                   _MyUserId,
                   out Conversation conversation,
                   out ChatFailedReason failedReason
               )
            )
                response = GetConversationResponse.Success(
                        conversation, failedReason, getConversationRequest.Ticket);
            else
                response = GetConversationResponse.Failed(failedReason, getConversationRequest.Ticket);
            _ClientEndpoint.SendObject(response);
        }*/
        private void GetPmConversation(TypeTicketedAndWholePayload message)
        {
            GetPmConversationRequest getConversationRequest = Json.Deserialize<GetPmConversationRequest>(message.JsonString);
            bool successful =
               ChatManager.Instance.GetPmConversation(
                   _MyUserId,
                   otherUserId: getConversationRequest.OtherUserId,
                   out long conversationId,
                   out ChatFailedReason failedReason
               );
            GetConversationIdResponse response = new GetConversationIdResponse(successful, conversationId, failedReason, getConversationRequest.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void GetWallConversation(TypeTicketedAndWholePayload message)
        {
            GetWallConversationRequest request = Json.Deserialize<GetWallConversationRequest>(message.JsonString);
            long myUserId = _MyUserId;
            bool successful =
               ChatManager.Instance.GetWallConversation(
                   myUserId,
                   userId: request.UserId,
                   out long conversationId,
                   out ChatFailedReason failedReason,
                   out int nodeId
               );
            if (successful)
            {
                _ActiveConversations.Add(myUserId, nodeId, conversationId, ConversationType.Wall);
            }
            GetConversationIdResponse response = new GetConversationIdResponse(successful, conversationId,
                    failedReason, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void GetWallCommentsConversation(TypeTicketedAndWholePayload message)
        {
            GetWallCommentsConversationRequest request = Json.Deserialize<GetWallCommentsConversationRequest>(message.JsonString);
            long myUserId = _MyUserId;
            bool successful = ChatManager.Instance.GetWallCommentsConversation(
                   myUserId,
                   request.WallConversationId,
                   request.WallMessageId,
                   out long conversationId,
                   out ChatFailedReason failedReason,
                   out int nodeId
               );
            if (successful)
            {
                _ActiveConversations.Add(myUserId, nodeId, conversationId, ConversationType.Comments);
            }
            GetConversationIdResponse response = new GetConversationIdResponse(successful,
                conversationId, failedReason, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        public void RemoveUserFromActiveConversations(TypeTicketedAndWholePayload message)
        {
            RemoveUserFromActiveConversationsRequest request = Json.Deserialize<RemoveUserFromActiveConversationsRequest>(message.JsonString);
            request.UserId = _MyUserId;
            _ActiveConversations.Remove(request.ConversationIds);
        }
        private void SendMessage(TypeTicketedAndWholePayload message)
        {
            SendMessageRequest request = Json.Deserialize<SendMessageRequest>(message.JsonString);
            request.UserId = _MyUserId;
            request.UserMultimediaItems = _PendingMultimediaItems
                .GetPendingUserMultimediaItems(request.UserMultimediaItems);
            SendMessageResponse response;
            if (
               ChatManager.Instance.SendMessage(
                   request,
                   out ChatFailedReason failedReason,
                   out ClientMessage replyMessage
               )
            )
                response = SendMessageResponse.Success(replyMessage, request.Ticket);
            else
                response = SendMessageResponse.Failed(failedReason, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void LoadMessagesHistory(TypeTicketedAndWholePayload message)
        {
            LoadMessagesHistoryRequest request = Json.Deserialize<LoadMessagesHistoryRequest>(message.JsonString);
            LoadMessagesHistoryResponse response;
            if (
                ChatManager.Instance.LoadMessagesHistory(
                    _MyUserId,
                    request.ConversationId,
                    request.ConversationType,
                    request.IdFromInclusive,
                    request.IdToExclusive,
                    request.NEntries,
                    out ClientMessage[] messages,
                    out MessageReaction[] messageReactions,
                    out MessageUserMultimediaItem[] messageUserMultimediaItems,
                    out ChatFailedReason failedReason,
                    request.MessageChildConversationOptions
                )
             )
                response = LoadMessagesHistoryResponse.Success(messages, messageReactions, messageUserMultimediaItems, failedReason, request.Ticket);
            else
                response = LoadMessagesHistoryResponse.Failed(failedReason, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void ReactToMessage(TypeTicketedAndWholePayload message)
        {
            ReactToMessage request = Json.Deserialize<ReactToMessage>(message.JsonString);
            request.MessageReaction.UserId = _MyUserId;
            ChatManager.Instance.ReactToMessage(
                request.ConversationId,
                request.ConversationType,
                request.MessageReaction
            );
        }
        private void UnreactToMessage(TypeTicketedAndWholePayload message)
        {
            UnreactToMessage request = Json.Deserialize<UnreactToMessage>(message.JsonString);
            request.MessageReaction.UserId = _MyUserId;
            ChatManager.Instance.UnreactToMessage(
                request.ConversationId,
                request.ConversationType,
                request.MessageReaction
            );
        }

        private void ModifyMessage(TypeTicketedAndWholePayload message)
        {
            ModifyMessage request = Json.Deserialize<ModifyMessage>(message.JsonString);
            if (request.Message == null) return;
            request.Message.UserId = _MyUserId;
            bool success = ChatManager.Instance.ModifyMessage(
                    request.ConversationId,
                    request.ConversationType,
                    request.Message
                );
            SuccessTicketedResponse response = new SuccessTicketedResponse(success, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }

        private void DeleteMessages(TypeTicketedAndWholePayload message)
        {
            DeleteMessagesRequest request = Json.Deserialize<DeleteMessagesRequest>(message.JsonString);
            long[] deletedIds = ChatManager.Instance.DeleteMessages(
                   _MyUserId,
                    request.ConversationId,
                    request.ConversationType,
                    request.MessageIds
                );
            DeleteMessagesResponse response = new DeleteMessagesResponse(
                deletedIds, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void HandlePmUploadMessagePicture(TypeTicketedAndWholePayload message)
        {
            UploadPmMessagePictureRequest request = Json
                .Deserialize<UploadPmMessagePictureRequest>(message.JsonString);
            MultimediaFailedReason? failedReason;
            failedReason = ChatMultimediaMesh.Instance.UploadPmMessagePicture(
                request.ConversationId, request.ConversationType, request.FileInfo, _MyUserId, _ClientEndpoint.SessionId, request.XRating,
                request.Description, out UserMultimediaItem userMultimediaItem);
            if (failedReason != null)
            {
                _ClientEndpoint.SendObject(UploadMultimediaResponse.Failed(failedReason, request.Ticket));
                return;
            }
            _PendingMultimediaItems.AddPendingMultimediaItemAndOverflow(userMultimediaItem);
            _ClientEndpoint.SendObject(UploadMultimediaResponse.Successful(userMultimediaItem!, request.Ticket));
        }
        private void HandlePmUploadMessageVideo(TypeTicketedAndWholePayload message)
        {
            UploadPmMessageVideoRequest request = Json
                .Deserialize<UploadPmMessageVideoRequest>(message.JsonString);
            MultimediaFailedReason? failedReason;
            failedReason = ChatMultimediaMesh.Instance.UploadPmMessageVideo(
                request.ConversationId, request.ConversationType, request.FileInfo, _MyUserId, _ClientEndpoint.SessionId, request.XRating,
                request.Description, out UserMultimediaItem userMultimediaItem);
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
        public void HandleSearchRooms(TypeTicketedAndWholePayload message)
        {
            SearchRoomsRequest request = Json
                .Deserialize<SearchRoomsRequest>(message.JsonString);
            ConversationWithTags[] conversationWithTagss = ChatManager.Instance.SearchRooms(request.Str);
            _ClientEndpoint.SendObject(new SearchRoomsResponse(conversationWithTagss, request.Ticket));
        }
        public void HandleSetSeenMessage(TypeTicketedAndWholePayload message)
        {
            SetSeenMessage request = Json
                .Deserialize<SetSeenMessage>(message.JsonString);
            ChatManager.Instance.SetSeenMessage(_MyUserId, request.ConversationId, request.MessageId);
        }
        public void UpdateMultimediaItemStatus(string multimediaToken, MultimediaItemStatus status)
        {
            _PendingMultimediaItems.UpdateMultimediaItemStatus(multimediaToken, status);
        }
        private void GetUserRooms(TypeTicketedAndWholePayload message)
        {
            GetUserRoomsRequest request = Json.Deserialize<GetUserRoomsRequest>(message.JsonString);
            request.MyUserId = _MyUserId;
            GetUserRoomsResponse response;
            if (ChatRoomsMesh.Instance.GetUserRooms(
                    _MyUserId,
                    request.Operation,
                    out string entriesSerialized
                ))
                response = GetUserRoomsResponse.Success(entriesSerialized, request.Ticket);
            else
                response = GetUserRoomsResponse.Failed(request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void ModifyUserRooms(TypeTicketedAndWholePayload message)
        {
            ModifyUserRoomsRequest request = Json.Deserialize<ModifyUserRoomsRequest>(message.JsonString);
            request.MyUserId = _MyUserId;
            bool success = ChatRoomsMesh.Instance.ModifyUserRooms(
                    _MyUserId,
                    request.ConversationId,
                    request.AddElseRemove,
                    request.Operations
                );
            SuccessTicketedResponse response = new SuccessTicketedResponse(success, request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void GetRoomSummarys(TypeTicketedAndWholePayload message)
        {
            GetRoomSummarysRequest request = Json.Deserialize<GetRoomSummarysRequest>(message.JsonString);
            GetRoomSummarysResponse response;
            if (ChatRoomsMesh.Instance.GetRoomSummarys(
                    request.ConversationIds, out RoomSummary[] roomSummarys
                ))
                response = GetRoomSummarysResponse.Success(roomSummarys, request.Ticket);
            else
                response = GetRoomSummarysResponse.Failed(request.Ticket);
            _ClientEndpoint.SendObject(response);
        }
        private void HandleRoomInvite(TypeTicketedAndWholePayload message)
        {
            RoomInviteRequest request = Json.Deserialize<RoomInviteRequest>(message.JsonString);
            InviteFailedReason? failedReason = ChatRoomsMesh.Instance.RoomInvite(
                request.ConversationId, request.OtherUserId, _MyUserId
            );
            _ClientEndpoint.SendObject(new RoomInviteResponse(failedReason, request.Ticket));
        }
        private void HandleAcceptRoomInvite(TypeTicketedAndWholePayload message)
        {
            AcceptRoomInviteRequest request = Json.Deserialize<AcceptRoomInviteRequest>(message.JsonString);
            JoinFailedReason? failedReason = ChatRoomsMesh.Instance.AcceptRoomInvite(
                request.ConversationId, _MyUserId
            );
            _ClientEndpoint.SendObject(new AcceptRoomInviteResponse(failedReason, request.Ticket));
        }
        private void HandleRejectRoomInvite(TypeTicketedAndWholePayload message)
        {
            RejectRoomInviteRequest request = Json.Deserialize<RejectRoomInviteRequest>(message.JsonString);
            bool success = ChatRoomsMesh.Instance.RejectRoomInvite(
                request.ConversationId, _MyUserId
            );
            _ClientEndpoint.SendObject(new SuccessTicketedResponse(success, request.Ticket));
        }
        private void HandleCancelRoomInvite(TypeTicketedAndWholePayload message)
        {
            CancelRoomInviteRequest request = Json.Deserialize<CancelRoomInviteRequest>(message.JsonString);
            bool success = ChatRoomsMesh.Instance.CancelRoomInvite(
                request.ConversationId, _MyUserId, request.UserId
            );
            _ClientEndpoint.SendObject(new SuccessTicketedResponse(success, request.Ticket));
        }
        private void HandleLeaveRoom(TypeTicketedAndWholePayload message)
        {
            LeaveRoomRequest request = Json.Deserialize<LeaveRoomRequest>(message.JsonString);
            RemoveRoomUserFailedReason? failedReason = ChatRoomsMesh.Instance.LeaveRoom(
                request.ConversationId, _MyUserId, request.AllowRemoveOnlyFullAdmin
            );
            _ClientEndpoint.SendObject(new LeaveRoomResponse(failedReason, request.Ticket));
        }
        private void HandleGetMyReceivedInvites(TypeTicketedAndWholePayload message)
        {
            GetMyReceivedInvitesRequest request = Json.Deserialize<GetMyReceivedInvitesRequest>(message.JsonString);
            bool success = ChatRoomsMesh.Instance.GetMyReceivedInvites(
                _MyUserId,
                out Invites invites
            );
            _ClientEndpoint.SendObject(new GetMyReceivedInvitesResponse(success, invites, request.Ticket));
        }
        private void HandleGetMySentInvites(TypeTicketedAndWholePayload message)
        {
            GetMySentInvitesRequest request = Json.Deserialize<GetMySentInvitesRequest>(message.JsonString);
            bool success = ChatRoomsMesh.Instance.GetMySentInvites(
                _MyUserId,
                out Invites invites
            );
            _ClientEndpoint.SendObject(new GetMySentInvitesResponse(success, invites, request.Ticket));
        }
        private void CreateRoom(TypeTicketedAndWholePayload message)
        {
            CreateRoomRequest request = Json.Deserialize<CreateRoomRequest>(message.JsonString);
            ChatRoomInfo chatRoomInfo = null;
            ChatFailedReason? failedReason = null;
            try
            {
                chatRoomInfo = ChatRooms.Instance.Create(request.Name, _MyUserId, request.Visibility);
            }
            catch (Exception ex)
            {
                failedReason = ChatFailedReason.ServerError;
                Logs.Default.Error(ex);
            }
            CreateRoomResponse response = new CreateRoomResponse(failedReason, chatRoomInfo, request.Ticket);
            string responseString;
            lock (chatRoomInfo)
            {
                responseString = Json.Serialize(response);
            }
            _ClientEndpoint.SendJSONString(responseString);
        }

        public void Dispose()
        {
            _RemoveClientMessageTypeMappings();
            _ActiveConversations.Dispose();
            _PendingMultimediaItems.Dispose();
        }
    }
}
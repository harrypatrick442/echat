using JSON;
using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Chat.Messages.Client.Messages;
using Core.Messages.Responses;
using Chat.Messages.Client;
using Chat.DataMemberNames.Messages;

namespace Chat
{
    public partial class ChatManager
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new Core.TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.ClientMessage, HandleSendMessage},
                    { InterserverMessageTypes.ChatSendMessageAsCoreServerForUsers, HandleSendMessageAsCoreServerForUsers},
                    { InterserverMessageTypes.ChatSendMessageToUsersDevices, HandleSendMessageToUsersDevices},
                    {InterserverMessageTypes.ChatGetPmConversation, HandleGetPmConversation},
                    {InterserverMessageTypes.ChatLoadMessagesHistory, HandleLoadMessagesHistory},
                    {InterserverMessageTypes.ChatReactToMessage, HandleReactToMessage},
                    {InterserverMessageTypes.ChatUnreactToMessage, HandleUnreactToMessage},
                    {InterserverMessageTypes.ChatModifyMessage, HandleModifyMessage},
                    {InterserverMessageTypes.ChatDeleteMessages, HandleDeleteMessage},
                    {InterserverMessageTypes.ChatGetWallConversation, HandleGetWallConversation},
                    {InterserverMessageTypes.ChatGetWallCommentsConversation, HandleGetWallCommentsConversation},
                    {InterserverMessageTypes.ChatRemoveUserFromActiveConversations, HandleRemoveUserFromActiveConversations},
                    {InterserverMessageTypes.ChatGetAdministrators, HandleGetAdministrators},
                    {InterserverMessageTypes.ChatFetchIndividualMessages, HandleFetchIndividualMessages},
                    {InterserverMessageTypes.ChatSetSeenMessage, HandleSetSeenMessage}
                }
            );
        }
        private void HandleFetchIndividualMessages(InterserverMessageEventArgs e)
        {
            FetchIndividualMessagesRequest request = e.Deserialize<FetchIndividualMessagesRequest>();
            try
            {
                List<FetchConversationIndividualMessagesResult> results = ChatManager.Instance.FetchIndividualMessages_Here(request.MyUserId, request.ConversationAndMessageIds);
                FetchIndividualMessagesResponse response = new FetchIndividualMessagesResponse(results.ToArray(), request.Ticket);
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleSetSeenMessage(InterserverMessageEventArgs e)
        {
            SetSeenMessage request = e.Deserialize<SetSeenMessage>();
            try
            {
                ChatManager.Instance.SetSeenMessage_Here(request.MyUserId, request.ConversationId, request.MessageId);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleLoadMessagesHistory(InterserverMessageEventArgs e)
        {
            LoadMessagesHistoryRequest request = e.Deserialize<LoadMessagesHistoryRequest>();
            LoadMessagesHistoryResponse response;
            try
            {
                if (LoadMessagesHistory_Here(request.MyUserId, request.ConversationId,
                    request.ConversationType,
                    request.IdFromInclusive, request.IdToExclusive, request.NEntries,
                    out ClientMessage[] messages, out MessageReaction[] reactions,
                    out MessageUserMultimediaItem[] userMultimediaItems,
                    out ChatFailedReason failedReason,
                    request.MessageChildConversationOptions))
                    response = LoadMessagesHistoryResponse.Success(messages, reactions, userMultimediaItems, failedReason, request.Ticket);
                else
                    response = LoadMessagesHistoryResponse.Failed(failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = LoadMessagesHistoryResponse.Failed(ChatFailedReason.ServerError, request.Ticket);
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
        private void HandleGetConversationSnapshots(InterserverMessageEventArgs e)
        {
            GetConversationSnapshotsRequest request = e.Deserialize<GetConversationSnapshotsRequest>();
            GetConversationSnapshotsResponse response;
            if (GetConversationSnapshots_Here(request.MyUserId,
                request.IdFromInclusive, request.IdToExclusive, request.NEntries,
                out ConversationSnapshot[] messages, out MessageReaction[] reactions,
                out MessageUserMultimediaItem[] userMultimediaItems))
                response = GetConversationSnapshotsResponse.Success(messages, reactions, userMultimediaItems, ChatFailedReason.None, request.Ticket);
            else
                response = GetConversationSnapshotsResponse.Failed(ChatFailedReason.ServerError, request.Ticket);
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleSendMessageAsCoreServerForUsers(InterserverMessageEventArgs e)
        {
            SendMessageAsCoreServerForUsersRequest request = e.Deserialize<SendMessageAsCoreServerForUsersRequest>();
            try
            {
                SendMessageAsCoreServerForUsers_Here(request.UserIds,
                    request.ReceivedMessage, request.UserIds.Take(4).ToArray());
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleSendMessageToUsersDevices(InterserverMessageEventArgs e)
        {
            SendMessageToUsersDevicesRequest request = e.Deserialize<SendMessageToUsersDevicesRequest>();
            try
            {
                _SendMessageToUsersDevices_Here(request.UserIds, request.ReceivedMessageJsonString);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleSendMessage(InterserverMessageEventArgs e)
        {
            SendMessageRequest request = e.Deserialize<SendMessageRequest>();
            SendMessageResponse response;
            try
            {
                if (SendMessage_Here(request,
                    out ChatFailedReason failedReason, out ClientMessage replyMessage))
                    response = SendMessageResponse.Success(replyMessage, request.Ticket);
                else
                    response = SendMessageResponse.Failed(failedReason, request.Ticket);
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                response = SendMessageResponse.Failed(ChatFailedReason.None, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        /*
        private void HandleGetConversation(InterserverMessageEventArgs e)
        {
            GetConversationInterserverRequest request = e.Deserialize<GetConversationInterserverRequest>();
            GetConversationResponse response;
            try
            {
                if (GetConversation_Here(request.ConversationId, request.MyUserId, 
                    out Conversation conversation, out ChatFailedReason failedReason))
                    response = GetConversationResponse.Success(conversation, 
                        failedReason, request.Ticket);
                else
                    response = GetConversationResponse.Failed(failedReason, request.Ticket);
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                response = GetConversationResponse.Failed(ChatFailedReason.None, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }*/
        private void HandleGetPmConversation(InterserverMessageEventArgs e)
        {
            GetPmConversationInterserverRequest request = e.Deserialize<GetPmConversationInterserverRequest>();
            GetConversationIdResponse response;
            try
            {
                long conversationId = GetPmConversation_Here(request.MyUserId, request.OtherUserId, out ChatFailedReason failedReason);
                response = new GetConversationIdResponse(true, conversationId, failedReason,
                    request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetConversationIdResponse.Failed(ChatFailedReason.ServerError, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleGetWallConversation(InterserverMessageEventArgs e)
        {
            GetWallConversationRequest request = e.Deserialize<GetWallConversationRequest>();
            GetConversationIdResponse response;
            try
            {
                long conversationId = GetWallConversation_Here(request.MyUserId, request.UserId, //TODO not needed
                                                                                                 out ChatFailedReason failedReason);
                response = new GetConversationIdResponse(true, conversationId, failedReason,
                    request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new GetConversationIdResponse(false, 0, ChatFailedReason.ServerError, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleReactToMessage(InterserverMessageEventArgs e)
        {
            ReactToMessage request = e.Deserialize<ReactToMessage>();
            try
            {
                ReactToMessage_Here(request.ConversationId, request.ConversationType, request.MessageReaction);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleRemoveUserFromActiveConversations(InterserverMessageEventArgs e)
        {
            RemoveUserFromActiveConversationsInternalRequest request = e.Deserialize<RemoveUserFromActiveConversationsInternalRequest>();
            try
            {
                RemoveUserFromActiveConversations_Here(request.UserId, request.ConversationTypeWithConversationIdss);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleUnreactToMessage(InterserverMessageEventArgs e)
        {
            UnreactToMessage request = e.Deserialize<UnreactToMessage>();
            try
            {
                UnreactToMessage_Here(request.ConversationId, request.ConversationType, request.MessageReaction);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleModifyMessage(InterserverMessageEventArgs e)
        {
            ModifyMessage request = e.Deserialize<ModifyMessage>();
            SuccessTicketedResponse response;
            try
            {
                ModifyMessage_Here(request.ConversationId, request.ConversationType, request.Message);
                response = new SuccessTicketedResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleDeleteMessage(InterserverMessageEventArgs e)
        {
            DeleteMessagesRequest request = e.Deserialize<DeleteMessagesRequest>();
            SuccessTicketedResponse response;
            try
            {
                DeleteMessages_Here(request.UserId, request.ConversationId, request.ConversationType, request.MessageIds, request.CanDeleteAnyMessage);
                response = new SuccessTicketedResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleGetWallCommentsConversation(InterserverMessageEventArgs e)
        {
            GetWallCommentsConversationRequest request = e.Deserialize<GetWallCommentsConversationRequest>();
            GetConversationIdResponse response;
            try
            {
                bool successful = GetWallCommentsConversation_Here(
                    request.MyUserId, request.WallConversationId, request.WallMessageId,
                    out long conversationId, out ChatFailedReason failedReason);
                response = new GetConversationIdResponse(successful, conversationId, failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new GetConversationIdResponse(false, 0, ChatFailedReason.ServerError, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleGetAdministrators(InterserverMessageEventArgs e)
        {
            GetAdministratorsRequest request = e.Deserialize<GetAdministratorsRequest>();
            GetAdministratorsResponse response;
            try
            {
                Administrator[]? administrators = GetAdministrators_Here(request.ConversationId,
                    request.MyUserId, out AdministratorsFailedReason? failedReason);
                response = new GetAdministratorsResponse(administrators, failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new GetAdministratorsResponse(null, AdministratorsFailedReason.ServerError, request.Ticket);
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
using JSON;
using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Core.Messages.Responses;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using Chat;

namespace Chat
{
    public partial class ChatRoomsMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new Core.TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.ChatGetUserRooms, HandleGetUserRooms},
                    {InterserverMessageTypes.ChatModifyUserRooms, HandleModifyUserRooms},
                    {InterserverMessageTypes.ChatGetRoomSummarys, HandleGetChatRoomSummarys},
                    {InterserverMessageTypes.ChatGetMostActiveRooms, HandleGetMostActiveRooms},
                    {InterserverMessageTypes.ChatGetMostActiveRoomsFromManager, HandleGetMostActiveRoomsFromManager},
                    {InterserverMessageTypes.ChatMostActiveRooms, HandleChatMostActiveRooms},
                    {InterserverMessageTypes.ChatRoomInvite, HandleRoomInvite},
                    {InterserverMessageTypes.ChatAddReceivedInvite, HandleAddReceivedInvite},
                    { InterserverMessageTypes.ChatRemoveReceivedInvite, HandleRemoveReceivedInvite},
                    { InterserverMessageTypes.ChatGetMyReceivedInvites, HandleGetMyReceivedInvites},
                    {InterserverMessageTypes.ChatAddSentInvite, HandleAddSentInvite},
                    { InterserverMessageTypes.ChatRemoveSentInvite, HandleRemoveSentInvite},
                    { InterserverMessageTypes.ChatGetMySentInvites, HandleGetMySentInvites},
                    {InterserverMessageTypes.ChatAcceptRoomInvite, HandleAcceptRoomInvite},
                    {InterserverMessageTypes.ChatRemoveInviteFromRoom, HandleRemoveInviteFromRoom},
                    {InterserverMessageTypes.ChatRemoveUserFromRoom, HandleRemoveUserFromRoom}
                }
            );
        }
        private void HandleRoomInvite(InterserverMessageEventArgs e)
        {
            RoomInviteRequest request = e.Deserialize<RoomInviteRequest>();
            RoomInviteResponse response;
            try
            {
                InviteFailedReason? failedReasson = RoomInvite_Here(request.ConversationId, request.OtherUserId, request.MyUserId);
                response = new RoomInviteResponse(failedReasson, request.Ticket);
            }   
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new RoomInviteResponse(InviteFailedReason.ServerError, request.Ticket);
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
        private void HandleAddReceivedInvite(InterserverMessageEventArgs e)
        {
            AddReceivedInviteRequest request = e.Deserialize<AddReceivedInviteRequest>();
            AddReceivedInviteResponse response;
            try
            {
                AddReceivedInvite_Here(request.ConversationId, request.UserIdBeingInvited, request.UserIdInviting);
                response = new AddReceivedInviteResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new AddReceivedInviteResponse(false, request.Ticket);
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
        private void HandleRemoveReceivedInvite(InterserverMessageEventArgs e)
        {
            RemoveReceivedInviteRequest request = e.Deserialize<RemoveReceivedInviteRequest>();
            RemoveReceivedInviteResponse response;
            try
            {
                RemoveReceivedInvite_Here(request.ConversationId, request.UserIdBeingInvited,
                    request.UserIdInviting, out long[] userIdsInvitingRemoved);
                response = new RemoveReceivedInviteResponse(true, userIdsInvitingRemoved, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new RemoveReceivedInviteResponse(false, null, request.Ticket);
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
        private void HandleGetMyReceivedInvites(InterserverMessageEventArgs e)
        {
            GetMyReceivedInvitesRequest request = e.Deserialize<GetMyReceivedInvitesRequest>();
            GetMyReceivedInvitesResponse response;
            try
            {
                Invites invites = GetMyReceivedInvites_Here(request.MyUserId);
                response = new GetMyReceivedInvitesResponse(true, invites, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new GetMyReceivedInvitesResponse(false, null, request.Ticket);
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
        private void HandleGetMySentInvites(InterserverMessageEventArgs e)
        {
            GetMySentInvitesRequest request = e.Deserialize<GetMySentInvitesRequest>();
            GetMySentInvitesResponse response;
            try
            {
                Invites invites = GetMySentInvites_Here(request.MyUserId);
                response = new GetMySentInvitesResponse(true, invites, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new GetMySentInvitesResponse(false, null, request.Ticket);
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
        private void HandleAcceptRoomInvite(InterserverMessageEventArgs e)
        {
            AcceptRoomInviteRequest request = e.Deserialize<AcceptRoomInviteRequest>();
            AcceptRoomInviteResponse response;
            try
            {
                JoinFailedReason? failedReason = AcceptRoomInvite_Here(request.ConversationId, request.UserId);
                response = new AcceptRoomInviteResponse(failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new AcceptRoomInviteResponse(JoinFailedReason.ServerError, request.Ticket);
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
        private void HandleAddSentInvite(InterserverMessageEventArgs e)
        {
            AddSentInviteRequest request = e.Deserialize<AddSentInviteRequest>();
            AddSentInviteResponse response;
            try
            {
                AddSentInvite_Here(request.ConversationId, request.UserIdBeingInvited, request.UserIdInviting);
                response = new AddSentInviteResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new AddSentInviteResponse(false, request.Ticket);
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
        private void HandleRemoveSentInvite(InterserverMessageEventArgs e)
        {
            RemoveSentInviteRequest request = e.Deserialize<RemoveSentInviteRequest>();
            RemoveSentInviteResponse response;
            try
            {
                RemoveSentInvite_Here(request.ConversationId, request.UserIdBeingInvited, request.UserIdInviting);
                response = new RemoveSentInviteResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new RemoveSentInviteResponse(false, request.Ticket);
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
        private void HandleGetUserRooms(InterserverMessageEventArgs e)
        {
            GetUserRoomsRequest request = e.Deserialize<GetUserRoomsRequest>();
            GetUserRoomsResponse response;
            try
            {
                response = GetUserRoomsResponse.Success(GetRooms_Here(request.MyUserId, request.Operation), request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetUserRoomsResponse.Failed(request.Ticket);
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
        private void HandleModifyUserRooms(InterserverMessageEventArgs e)
        {
            ModifyUserRoomsRequest request = e.Deserialize<ModifyUserRoomsRequest>();
            SuccessTicketedResponse response;
            try
            {
                ModifyUserRooms_Here(request.MyUserId, request.ConversationId, 
                    request.AddElseRemove, request.Operations);
                response = new SuccessTicketedResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, request.Ticket);
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
        private void HandleGetChatRoomSummarys(InterserverMessageEventArgs e)
        {
            GetRoomSummarysRequest request = e.Deserialize<GetRoomSummarysRequest>();
            GetRoomSummarysResponse response;
            try
            {
                RoomSummary[] summarys= GetChatRoomSummarys_Here(request.ConversationIds);
                response = GetRoomSummarysResponse.Success(summarys, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetRoomSummarysResponse.Failed(request.Ticket);
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
        private void HandleGetMostActiveRooms(InterserverMessageEventArgs e)
        {
            GetMostActiveRoomsRequest request = e.Deserialize<GetMostActiveRoomsRequest>();
            GetMostActiveRoomsResponse response;
            try
            {
                RoomActivity[] mostActiveRooms = GetMostActiveRooms_Here(request.NMostActive);
                response = GetMostActiveRoomsResponse.Success(mostActiveRooms, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetMostActiveRoomsResponse.Failed(request.Ticket);
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
        private void HandleGetMostActiveRoomsFromManager(InterserverMessageEventArgs e)
        {
            GetMostActiveRoomsFromManagerRequest request = e.Deserialize<GetMostActiveRoomsFromManagerRequest>();
            GetMostActiveRoomsFromManagerResponse response;
            try
            {
                string mostActiveRooms = MostActiveChatRoomsWatcher.Instance.GetMostActiveRoomsAllServers();
                response = GetMostActiveRoomsFromManagerResponse.Success(mostActiveRooms, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetMostActiveRoomsFromManagerResponse.Failed(request.Ticket);
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
        private void HandleRemoveInviteFromRoom(InterserverMessageEventArgs e)
        {
            RemoveInviteFromRoomRequest request = e.Deserialize<RemoveInviteFromRoomRequest>();
            SuccessTicketedResponse response;
            try
            {
                RemoveInviteFromRoom_Here(request.ConversationId, request.UserIdBeingInvited, request.UserIdInviting);
                response = new SuccessTicketedResponse(true, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, request.Ticket);
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
        private void HandleRemoveUserFromRoom(InterserverMessageEventArgs e)
        {
            RemoveUserFromRoomRequest request = e.Deserialize<RemoveUserFromRoomRequest>();
            RemoveUserFromRoomResponse response;
            try
            {
                RemoveRoomUserFailedReason? failedReason =  RemoveUserFromRoom_Here(request.ConversationId,
                    request.UserId, request.AllowRemoveOnlyFullAdmin);
                response = new RemoveUserFromRoomResponse(failedReason, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new RemoveUserFromRoomResponse(RemoveRoomUserFailedReason.ServerError, request.Ticket);
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
        private void HandleChatMostActiveRooms(InterserverMessageEventArgs e)
        {
            MostActiveChatrooms mostActiveRooms = e.Deserialize<MostActiveChatrooms>();
            MostActiveChatRoomsWatcher.Instance.UpdateAsNonManager(mostActiveRooms);
        }
    }
}
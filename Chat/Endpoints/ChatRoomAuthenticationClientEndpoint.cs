using Core.Messages;
using System.Net;
using Logging;
using Core.Handlers;
using Core.InterserverComs;
using JSON;
using Core.Interfaces;
using Chat;
using Core.Chat;
using Chat.Messages.Client.Messages;

namespace Core.Authentication
{
    public class ChatRoomAuthenticationClientEndpoint
    {
        private static readonly string NEEDS_PASSWORD_MESSAGE = Json
            .Serialize(new IncorrectPasswordMessage());
        private IClientEndpointLight _Endpoint;
        private IPAddress _IPAddress;
        private long _ConversationId, _UserId;
        private DelegateEnterRoom _EnterRoom;
        private Action _RemoveMappings, _Dispose;
        public ChatRoomAuthenticationClientEndpoint(
            IClientEndpointLight endpoint,
            long conversationId,
            long userId,
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IPAddress ipAddress,
            DelegateEnterRoom enterRoom,
            Action dispose
        )
        {
            _Endpoint = endpoint;
            _IPAddress = ipAddress;
            _ConversationId = conversationId;
            _UserId = userId;
            _EnterRoom = enterRoom;
            _Dispose = dispose;
            _RemoveMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { global::MessageTypes.MessageTypes.ChatAttemptEnterRoom, HandleAttemptEnterRoom}
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomPasswordProvided"></param>
        /// <returns>doOuterReturn</returns>
        private void HandleAttemptEnterRoom(TypeTicketedAndWholePayload t)
        {
            AttemptEnterRoomMessage request = Json.Deserialize<AttemptEnterRoomMessage>(t.JsonString);
            try
            {
                ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(_ConversationId);
                FailedEnterRoomReason failedReason = FailedEnterRoomReason.ServerError; ;
                JoinFailedReason? joinFailedReason = null;
                if (chatRoom != null)
                {
                    switch (chatRoom.Visibility)
                    {
                        case RoomVisibility.Public:
                        case RoomVisibility.InviteOnlyByAnyone:
                            if (TryJoinIfNecessary(chatRoom, ref joinFailedReason))
                                return;
                            failedReason = FailedEnterRoomReason.NotMember;
                            break;
                        case RoomVisibility.InviteOnlyByAdmins:
                            if (TryJoinIfNecessary(chatRoom, ref joinFailedReason))
                                return;
                            failedReason = FailedEnterRoomReason.NotMember;
                            break;
                        case RoomVisibility.Closed:
                            if (chatRoom.HasJoinedUser(_UserId))
                            {
                                _EnterRoom(chatRoom, _UserId);
                                return;
                            }
                            failedReason = FailedEnterRoomReason.NotMember;
                            joinFailedReason = JoinFailedReason.Closed;
                            break;
                        default:
                            Logs.Default.Error("defaulted. This should never happen");
                            if (TryJoinIfNecessary(chatRoom, ref joinFailedReason))
                                return;
                            failedReason = FailedEnterRoomReason.NotMember;
                            break;
                    }
                }
                else {
                    failedReason = FailedEnterRoomReason.NoLongerExists;
                }
                _Endpoint.SendObject(new FailedEnterRoomMessage(failedReason, joinFailedReason, chatRoom.Visibility));
                _Dispose();
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private bool TryJoinIfNecessary(ChatRoom chatRoom, ref JoinFailedReason? joinFailedReason) {
            bool userHasJoined = chatRoom.HasJoinedUser(_UserId);
            if (userHasJoined)
            {
                ChatRoomsMesh.Instance.ModifyUserRooms(_UserId, chatRoom.ConversationId, true, UserRoomsOperation.Recent);
                _EnterRoom(chatRoom, _UserId);
                return true;
            }
            joinFailedReason = chatRoom.Join(_UserId);
            if (joinFailedReason== null)
            {
                ChatRoomsMesh.Instance.ModifyUserRooms(_UserId, chatRoom.ConversationId, true, UserRoomsOperation.Joined, UserRoomsOperation.Recent);
                _EnterRoom(chatRoom, _UserId);
                return true;
            }
            return false;
        }
        public void Dispose()
        {
            _RemoveMappings();
        }
    }
}

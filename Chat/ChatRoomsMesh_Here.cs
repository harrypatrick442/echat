using Chat.Messages.Client.Messages;
using Chat.Messages.Client.Requests;
using Core.DAL;
using JSON;
using Logging;
using UserRoutedMessages;

namespace Chat
{
    public partial class ChatRoomsMesh
    {
        public string GetRooms_Here(long myUserId, UserRoomsOperation operation)
        
        {
            if (operation.Equals(UserRoomsOperation.Popular))
            {
                return MostActiveChatRoomsWatcher.Instance.GetSerialized();
            }
            UserRooms userRooms = _DalUserRooms.Get(myUserId);
            if (userRooms == null) return null;
            lock (userRooms)
            {
                switch (operation)
                {
                    case UserRoomsOperation.Pinned:
                        return Json.Serialize(userRooms.Pinned);
                    case UserRoomsOperation.Recent:
                        return Json.Serialize(userRooms.Recent);
                    case UserRoomsOperation.Mine:
                        return Json.Serialize(userRooms.Mine);
                    case UserRoomsOperation.Joined:
                        return Json.Serialize(userRooms.Joined);//Copy to take outside lock
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public void ModifyUserRooms_Here(long myUserId, long conversationId, bool addElseRemove,
            UserRoomsOperation[] operations)
        {
            _DalUserRooms.Modify(myUserId, (userRooms) =>
            {
                foreach (UserRoomsOperation operation in operations)
                {
                    if (addElseRemove)
                    {
                        if (userRooms == null)
                            userRooms = new UserRooms();
                        switch (operation)
                        {
                            case UserRoomsOperation.Mine:
                                userRooms.AddMine(conversationId);
                                break;
                            case UserRoomsOperation.Pinned:
                                userRooms.Pin(conversationId);
                                break;
                            case UserRoomsOperation.Recent:
                                userRooms.AddRecent(conversationId);
                                break;
                            case UserRoomsOperation.Joined:
                                userRooms.AddJoined(conversationId);
                                break;
                        }
                    }
                    else
                    {
                        if (userRooms == null) return userRooms;
                        switch (operation)
                        {
                            case UserRoomsOperation.Mine:
                                userRooms.RemoveMine(conversationId);
                                break;
                            case UserRoomsOperation.Pinned:
                                userRooms.Unpin(conversationId);
                                break;
                            case UserRoomsOperation.Recent:
                                userRooms.RemoveRecent(conversationId);
                                break;
                            case UserRoomsOperation.Joined:
                                userRooms.RemoveJoined(conversationId);
                                break;
                        }
                    }
                }
                UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                    new ModifyUserRooms(conversationId, addElseRemove, operations),
                    new long[] { myUserId }
                );
                return userRooms;
            });
        }
        private RoomActivity[] GetMostActiveRooms_Here(int nMostActive)
        {
            return MostActiveChatRoomsWatcher.Instance.GetForThisServer(nMostActive);
        }
        private RoomSummary[] GetChatRoomSummarys_Here(long[] conversationIds)
        {
            return conversationIds.Select(c => DalChatRoomInfos.Instance.Get(c)).Where(i => i != null).Select(i => i.ToSummary()).ToArray();
        }
        private InviteFailedReason? RoomInvite_Here(long conversationId, long otherUserId, long myUserId)
        {
            ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
            if (chatRoom == null)
                return InviteFailedReason.ServerError;
            InviteFailedReason? failedReason = chatRoom.Invite(myUserId, otherUserId);
            if (failedReason != null) return failedReason;
            try
            {
                AddReceivedInvite(conversationId, otherUserId, myUserId);
                try
                {
                    AddSentInvite(conversationId, otherUserId, myUserId);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return InviteFailedReason.ServerError;
            }
            return null;
        }
        private void AddReceivedInvite_Here(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            DalInvites.Instance.AddReceivedInvite(conversationId, userIdBeingInvited, userIdInviting);
        }
        private bool RemoveReceivedInvite_Here(long conversationId, long userIdBeingInvited, long? userIdInviting, out long[] userIdsInvitingRemoved)
        {
            return DalInvites.Instance.RemoveReceivedInvite(conversationId, userIdBeingInvited, userIdInviting, out userIdsInvitingRemoved);
        }
        private void RemoveInviteFromRoom_Here(long conversationId, long userIdBeingInvited, long? userIdInviting)
        {
            ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
            if (chatRoom == null) return;
            if (userIdInviting != null)
            {
                chatRoom.RemoveInvite(userIdBeingInvited, (long)userIdInviting);
                return;
            }
            chatRoom.RemoveInvite(userIdBeingInvited);
        }
        private Invites GetMyReceivedInvites_Here(long myUserId)
        {
            return DalInvites.Instance.GetMyReceivedInvites(myUserId);
        }
        private void AddSentInvite_Here(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            DalInvites.Instance.AddSentInvite(conversationId, userIdBeingInvited, userIdInviting);
        }
        private void RemoveSentInvite_Here(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            DalInvites.Instance.RemoveSentInvite(conversationId, userIdBeingInvited, userIdInviting);
        }
        private RemoveRoomUserFailedReason? RemoveUserFromRoom_Here(long conversationId, long userId,
            bool allowRemoveOnlyFullAdmin)
        {
            ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
            if (chatRoom == null) return null;
            chatRoom.RemoveUser(userId, allowRemoveOnlyFullAdmin,
            out RemoveRoomUserFailedReason? failedReason);
            return failedReason;
        }
        private JoinFailedReason? AcceptRoomInvite_Here(
            long conversationId, long myUserId)
        {
            try
            {
                ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
                if (chatRoom == null)
                    return JoinFailedReason.ServerError;
                JoinFailedReason? failedReason =  chatRoom.Join(myUserId); 
                return failedReason;
            }
            catch (Exception ex) {
                return JoinFailedReason.ServerError;
            }
        }
        private Invites GetMySentInvites_Here(long myUserId)
        {
            return DalInvites.Instance.GetMySentInvites(myUserId);
        }
    }
}
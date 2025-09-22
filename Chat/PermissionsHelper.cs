using Logging;
using Core.DAL;
using JSON;
using Core.Threading;
using Nodes;
using InterserverComs;
using Core.Interfaces;
using NodeAssignedIdRanges;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client;
using UserRoutedMessages;
using Chat.Interfaces;
using MultimediaServerCore;
using UserRouting;
using KeyValuePairDatabases;
using UsersEnums;
using Users.DAL;
using Users;
using Microsoft.VisualBasic;
using Core.Chat;

namespace Chat
{
    public static class PermissionsHelper
    {
        public static bool CheckHasPermissionsAddUserAndGetUsers(Wall wall, long myUserId, 
            out IConversation useUserIds, out ChatFailedReason failedReason, bool addActiveUser)
        {
            useUserIds = wall;
            if (_CheckHasPermissionsAndGetUsers(wall, myUserId, out failedReason))
            {
                if (addActiveUser)
                    wall.AddActiveUser(myUserId);
                return true;
            }
            return false;
        }
        public static bool _CheckHasPermissionsAndGetUsers(Wall wall, long myUserId, out ChatFailedReason failedReason)
        {
            failedReason = ChatFailedReason.UserNotIncluded;
            if (wall == null)
            {
                return false;
            }
            if (wall.OwnerUserId == myUserId)
            {
                return true;
            }
            switch (wall.VisibleTo)
            {
                case VisibleTo.Public:
                    failedReason = ChatFailedReason.None;
                    return true;
                case VisibleTo.None:
                    return false;
                default:
                    Associates associates = DalAssociates.Instance.GetUsersAssociates(wall.OwnerUserId);
                    if (associates == null)
                    {
                        return false;
                    }
                    if (!associates.TryGet(myUserId, out Associate associate))
                    {
                        return false;
                    }

                    if (((int)associate.AssociateType & (int)wall.VisibleTo) > 0)
                    {

                        failedReason = ChatFailedReason.None;
                        return true;
                    }
                    return false;
            }
        }
        public static bool CheckHasPermissionsAddUserAndGetUsers(Comments comments, long myUserId, out IConversation useUserIds, out ChatFailedReason failedReason, bool addActiveUser)
        {
            switch (comments.ScopeType) {
                case CommentsScopeType.Wall:
                    useUserIds = comments;
                    if (_CheckHasPermissionsAndGetUsers(
                        DalWalls.Instance.GetWallByConversationId(comments.ScopeId),
                        myUserId,
                        out failedReason
                    )) {
                        if(addActiveUser)
                            comments.AddActiveUser(myUserId);
                        return true;
                    }
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }
        private static bool CheckHasPermissionsAddUserAndGetUsers(Pm pm, long myUserId, out IConversation useUserIds, out ChatFailedReason failedReason)
        {
            useUserIds = pm;
            if (pm == null)
            {
                failedReason = ChatFailedReason.UserNotIncluded;
                return false;
            }
            if (pm.LowestUserId == myUserId || pm.HighestUserId == myUserId) {
                failedReason = ChatFailedReason.None;
                return true;
            }
            failedReason = ChatFailedReason.UserNotIncluded;
            return false;
        }
        public static bool CheckHasPermissionsAddUserAndGetUsers(long conversationId, ConversationType conversationType, 
            long myUserId, out IConversation useUserIds, out ChatFailedReason failedReason, bool addActiveUser = false)
        {
            switch (conversationType)
            {
                case ConversationType.PublicChatroom:
                    throw new Exception("Shouldnt be taking this route");
                case ConversationType.Pm:
                    return CheckHasPermissionsAddUserAndGetUsers(DalPms.Instance.GetPm(conversationId), 
                        myUserId, out useUserIds, out failedReason);
                case ConversationType.GroupChat:
                    throw new NotImplementedException();
                case ConversationType.Wall:
                    return CheckHasPermissionsAddUserAndGetUsers(
                        DalWalls.Instance.GetWallByConversationId(conversationId),
                        myUserId, out useUserIds, out failedReason, addActiveUser);
                case ConversationType.Comments:
                    return CheckHasPermissionsAddUserAndGetUsers(
                        DalComments.Instance.GetCommentsByConversationId(conversationId),
                        myUserId, out useUserIds, out failedReason, addActiveUser);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
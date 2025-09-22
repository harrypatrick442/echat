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
using Users;
using Users.DAL;
using Core.Chat;
using System.Xml.Linq;
using GlobalConstants;
using HashTags;
using NotificationsCore.Enums;
using Core.DTOs;
using Users.Messages.Client;

namespace Chat
{
    public partial class ChatManager
    {
        public long GetPmConversation_Here(long myUserId, long otherUserId,
            out ChatFailedReason failedReason)
        {
            failedReason = ChatFailedReason.None;
            Pm pm = DalPms.Instance.GetOrCreatePm(myUserId, otherUserId);
            return pm.ConversationId;
        }
        public long GetWallConversation_Here(long myUserId, long userId, out ChatFailedReason failedReason)
        {
            UserProfile userProfile = DalUserProfiles.Instance.GetUserProfile(userId);
            if (userProfile == null)
            {
                failedReason = ChatFailedReason.ServerError;
                return -1;
            }
            long wallConversationId = userProfile.WallConversationId;
            Wall wall = null;
            if (wallConversationId <= 0
                || (wall = DalWalls.Instance.GetWallByConversationId(wallConversationId)) == null)
            {
                if (myUserId != userId)
                {
                    failedReason = ChatFailedReason.ServerError;
                    return -1;
                }
                DalUserProfiles.Instance.ModifyUserProfile(userId, (userProfileBeingModified) => {
                    if (userProfileBeingModified == null)
                        throw new Exception($"{nameof(userProfileBeingModified)} Shouldnt be null here");
                    wallConversationId = userProfileBeingModified.WallConversationId;
                    if (wallConversationId <= 0
                        || (wall = DalWalls.Instance.GetWallByConversationId(wallConversationId)) == null)
                    {
                        wallConversationId = ConversationIdSource.Instance.NextId();
                        wall = new Wall(wallConversationId, UsersEnums.VisibleTo.Public, userId);
                        DalWalls.Instance.SetWall(wallConversationId, wall);
                        userProfileBeingModified.WallConversationId = wallConversationId;
                    }
                    return userProfileBeingModified;
                });
                if (wallConversationId <= 0)
                {
                    failedReason = ChatFailedReason.ServerError;
                    return -1;
                }
            }
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(wall, myUserId, out IConversation ignore, out failedReason, true))
            {
                return -1;
            }
            failedReason = ChatFailedReason.None;
            return wall.ConversationId;
        }
        public bool GetWallCommentsConversation_Here(
                long myUserId,
                long wallConversationId,
                long wallMessageId,
                out long conversationId,
                out ChatFailedReason failedReason)
        {
            IDalMessages dalMessages = DalMessages.ForConversationType(ConversationType.Wall);
            //Will be returned with the message once in place.
            long? conversationIdInternal = dalMessages.GetChildConversationIdForMessage(
                wallConversationId, wallMessageId);
            Comments comments = null;
            if (conversationIdInternal != null)
            {
                conversationId = (long)conversationIdInternal;
                Func<Comments, bool> commentsInalid = (c) => c == null ||
                    c.ConversationId != (long)conversationIdInternal
                    || c.OwnerUserId <= 0 || c.ScopeId != wallConversationId
                    || c.ScopeType != CommentsScopeType.Wall;
                comments = DalComments.Instance.GetCommentsByConversationId(conversationId);
                if (commentsInalid(comments))
                {
                    DalWalls.Instance.ModifyWall(wallConversationId, (wall, save) =>
                    {
                        if (wall == null)
                        {
                            throw new NullReferenceException($"{nameof(Wall)} with {nameof(wallConversationId)} {wallConversationId} was null");
                        }
                        if (!commentsInalid(comments)) return;
                        comments = new Comments((long)conversationIdInternal, CommentsScopeType.Wall, wall.OwnerUserId, wall.ConversationId);
                        DalComments.Instance.SetComments((long)conversationIdInternal, comments);
                    });
                }
            }
            else
            {
                DalWalls.Instance.ModifyWall(wallConversationId, (wall, save) =>
                {
                    conversationIdInternal = dalMessages.GetChildConversationIdForMessage(
                        wallConversationId, wallMessageId);
                    if (conversationIdInternal != null)
                    {
                        return;
                    }
                    if (wall == null)
                    {
                        throw new NullReferenceException($"{nameof(Wall)} with {nameof(wallConversationId)} {wallConversationId} was null");
                    }
                    //For now on same node but can be put
                    //on another node with load balancing
                    comments = new Comments(ConversationIdSource.Instance.NextId(),
                        CommentsScopeType.Wall, wall.OwnerUserId, wall.ConversationId);
                    dalMessages.SetChildConversationIdForMessage(wallConversationId, wallMessageId,
                        comments.ConversationId);
                    DalComments.Instance.SetComments(comments.ConversationId, comments);
                    conversationIdInternal = comments.ConversationId;
                });
            }
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(comments, myUserId, out IConversation usingUserIds, out failedReason, true))
            {
                conversationId = 0;
                return false;
            }
            failedReason = ChatFailedReason.None;
            conversationId = (long)conversationIdInternal;
            return true;
        }
        /*
        public bool GetConversation_Here(
            long conversationId, long myUserId,
            out Conversation conversation,
            out ChatFailedReason failedReason)
        {

            conversation = GetConversation_Here(conversationId, myUserId, out failedReason);
            if (failedReason != ChatFailedReason.None)
            {
                return false;
            }
            return true;
        }*/
        private ClientMessage[] GetLatestMessages_Here(long conversationId, ConversationType conversationType,
            out MessageReaction[] reactions, out MessageUserMultimediaItem[] messageUserMultimediaItems,
            MessageChildConversationOptions messageChildConversationOptions)
        {
            IDalMessages dalMessages = DalMessages.ForConversationType(conversationType);
            switch (conversationType)
            {
                case ConversationType.Pm:
                    return dalMessages.ReadFromEnd(conversationId,
                        GlobalConstants.Lengths.N_MESSAGES_LOAD_PM, out reactions, out messageUserMultimediaItems,
                        messageChildConversationOptions);
                case ConversationType.GroupChat:
                    return dalMessages.ReadFromEnd(conversationId,
                        GlobalConstants.Lengths.N_MESSAGES_LOAD_GROUP_CHAT, out reactions, out messageUserMultimediaItems,
                        messageChildConversationOptions);
                case ConversationType.PublicChatroom:
                    throw new NotImplementedException();
                //return DalLatestMessages.Instance.ReadAll(conversationId, out reactions);
                default:
                    throw new NotImplementedException();
            }
        }/*
        public Conversation GetConversation_Here(long conversationId, long myUserId,
            out ChatFailedReason failedReason)
        {
            Conversation conversation = DalConversations.Instance.GetConversation(conversationId);
            if (conversation == null)
            {
                failedReason = ChatFailedReason.ConversationDoesNotExist;
                return null;
            }
            if (!PermissionsHelper.HasPermissions(conversation, myUserId))
            {
                failedReason = ChatFailedReason.UserNotIncluded;
                return null;
            }
            failedReason = ChatFailedReason.None;
            return conversation;
        }*/
        public bool SendMessage_Here(
            ClientMessage clientMessage, out ChatFailedReason sendMessageFailedReason, out ClientMessage replyMessage)
        {
            replyMessage = null;
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(
                clientMessage.ConversationId, clientMessage.ConversationType,
                clientMessage.UserId, out IConversation conversation, out sendMessageFailedReason))
            {
                return false;
            }
            IDalMessages dalMessages = DalMessages.ForConversationType(clientMessage.ConversationType);
            dalMessages.Append(clientMessage.ConversationId, clientMessage, out replyMessage);
            clientMessage.ReplyMessage = replyMessage;
            MentionsHelper.SendMentionsToServersForMentionedUsers(clientMessage, isUpdate: false);
            long[] userIds = conversation?.UserIdsToArray();
            SendMessageToCoreServersForUsers(
                userIds, clientMessage);
            sendMessageFailedReason = ChatFailedReason.None;
            return true;
        }
        public bool LoadMessagesHistory_Here(long myUserId, long conversationId, 
            ConversationType conversationType, long? idFromInclusive, long? idToExclusive, 
            int? nEntries, out ClientMessage[] messages, out MessageReaction[] reactions,
            out MessageUserMultimediaItem[] messageUserMultimediaItems, out ChatFailedReason failedReason,
            MessageChildConversationOptions messageChildConversationOptions)
        {
            messages = null;
            failedReason = ChatFailedReason.None;
            reactions = null;
            messageUserMultimediaItems = null;
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(conversationId, conversationType,
                myUserId, out IConversation conversation, out failedReason))
            {
                return false;
            }
            int maxNMessages = ConstrainNEntries(conversationType, nEntries);
            messages = DalMessages.ForConversationType(conversationType)
                .ReadRange(conversationId, maxNMessages, idFromInclusive, idToExclusive, out reactions,
                out messageUserMultimediaItems, messageChildConversationOptions);
            return true;
        }
        public List<FetchConversationIndividualMessagesResult> FetchIndividualMessages_Here(
            long myUserId, IEnumerable<ConversationAndMessageIds> conversationAndMessageIdss)
        {
            List<FetchConversationIndividualMessagesResult> results = 
                new List<FetchConversationIndividualMessagesResult>(conversationAndMessageIdss.Count());
            foreach(ConversationAndMessageIds conversationAndMessageIds in conversationAndMessageIdss)
            {
                long conversationId = conversationAndMessageIds.ConversationId;
                try
                {
                    ChatFailedReason failedReason = ChatFailedReason.None;
                    ConversationType conversationType = conversationAndMessageIds.ConversationType;
                    switch (conversationType)
                    {
                        case ConversationType.PublicChatroom:
                            ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
                            if (chatRoom == null)
                            {    
                                results.Add(new FetchConversationIndividualMessagesResult(
                                    conversationId, failedReason = ChatFailedReason.ConversationDoesNotExist)
                                );
                                continue;
                            }
                            if (!chatRoom.HasJoinedUser(myUserId))
                            {
                                results.Add(new FetchConversationIndividualMessagesResult(
                                    conversationId, failedReason = ChatFailedReason.UserNotIncluded)
                                );
                                continue;
                            }
                            break;
                        default:
                            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(
                                conversationId,
                                conversationType,
                                myUserId, out IConversation conversation, out failedReason))
                            {
                                results.Add(new FetchConversationIndividualMessagesResult(
                                    conversationId, failedReason)
                                );
                                continue;
                            }
                            break;
                    }
                    IDalMessages dalMessages = DalMessages.ForConversationType(conversationAndMessageIds.ConversationType);
                    ClientMessage[] messages = dalMessages.ReadIndividualMessages(conversationId, conversationAndMessageIds.MessageIds);
                    results.Add(new FetchConversationIndividualMessagesResult(
                        conversationId, conversationType, messages)
                    );
                }
                catch (Exception ex) {
                    Logs.Default.Error(ex);
                    results.Add(new FetchConversationIndividualMessagesResult(
                        conversationId,
                        ChatFailedReason.ServerError
                    ));
                }
            }
            return results;
        }
        public bool GetConversationSnapshots_Here(long myUserId, long? idFromInclusive, long? idToExclusive, 
            int? nEntries, out ConversationSnapshot[] messages, out MessageReaction[] reactions,
            out MessageUserMultimediaItem[] messageUserMultimediaItems)
        {
            try
            {
                messages = ConversationSnapshotsManager.Instance.GetUserConversationSnapshotsLocal(
                   myUserId, idFromInclusive, idToExclusive, nEntries,
               out reactions,
               out messageUserMultimediaItems);
               return true;
            }
            catch(Exception ex)
            {
                Logs.Default.Error(ex);
                messages = null;
                reactions = null;
                messageUserMultimediaItems = null;
                return false;
            }
        }
        public long[] DeleteMessages_Here(long myUserId, long conversationId, 
            ConversationType conversationType, long[] messageIds, bool canDeleteAnyMessage)
        {
            //TODO might wish to put null check in here but for now will throw exception
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(conversationId, conversationType,
                myUserId, out IConversation useUserIds, out ChatFailedReason failedReason))
            {
                return null;
            }
            IDalMessages dalMessages = DalMessages.ForConversationType(conversationType);
            List<string> multimediaTokensDeleted;
            long[] deletedMessageIds = canDeleteAnyMessage
                ? dalMessages.DeleteAny(conversationId, messageIds, out multimediaTokensDeleted)
                : dalMessages.Delete(myUserId, conversationId, messageIds, out multimediaTokensDeleted);
            if (multimediaTokensDeleted != null)
            {
                MultimediaServerMesh.Instance.Delete(multimediaTokensDeleted);
            }
            if (!conversationType.Equals(ConversationType.PublicChatroom))
            {
                UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                    new DeletedMessages(deletedMessageIds, conversationId), useUserIds?.UserIdsToArray());
            }
            return deletedMessageIds;
        }
        public void ModifyMessage_Here(long conversationId, ConversationType conversationType,
            ClientMessage message)
        {
            //TODO might wish to put null check in here but for now will throw exception
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(conversationId, conversationType,
                message.UserId, out IConversation useUserIds, out ChatFailedReason failedReason))
            {
                return;
            }
            IDalMessages dalMessages = DalMessages.ForConversationType(conversationType);
            dalMessages.Modify(conversationId, message, out List<string> multimediaTokensDeleted);
            if (multimediaTokensDeleted != null)
            {
                MultimediaServerMesh.Instance.Delete(multimediaTokensDeleted);
            }
            MentionsHelper.SendMentionsToServersForMentionedUsers(message, isUpdate: true);
            if (!conversationType.Equals(ConversationType.PublicChatroom))
            {
                UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(new ModifyMessage(conversationId, message), useUserIds?.UserIdsToArray());
            }
        }
        public void ReactToMessage_Here(long conversationId, ConversationType conversationType, MessageReaction reaction)
        {
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(conversationId, conversationType,
                reaction.UserId, out IConversation useUserIds, out ChatFailedReason failedReason))
            {
                return;
            }
            DalMessages.ForConversationType(conversationType).AddReaction(conversationId, reaction);
            if (!conversationType.Equals(ConversationType.PublicChatroom))
            {
                UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                    new ReactToMessage(conversationId, conversationType, reaction), useUserIds?.UserIdsToArray());
            }
        }
        public void UnreactToMessage_Here(long conversationId, ConversationType conversationType, MessageReaction reaction)
        {
            //TODO might wish to put null check in here but for now will throw exception
            if (!PermissionsHelper.CheckHasPermissionsAddUserAndGetUsers(conversationId, conversationType, 
                reaction.UserId, out IConversation usingUserIds, out ChatFailedReason failedReason))
            {
                return;
            }
            DalMessages.ForConversationType(conversationType).RemoveReaction(conversationId, reaction);
            if (!conversationType.Equals(ConversationType.PublicChatroom))
            {
                UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                    new UnreactToMessage(conversationId, reaction), usingUserIds?.UserIdsToArray());
            }
        }
        private int ConstrainNEntries(ConversationType conversationType, int? nEntries)
        {
            int maxNEntries;
            switch (conversationType)
            {
                case ConversationType.Pm:
                    maxNEntries = GlobalConstants.Lengths.N_MESSAGES_LOAD_PM;
                    break;
                case ConversationType.GroupChat:
                    maxNEntries = GlobalConstants.Lengths.N_MESSAGES_LOAD_GROUP_CHAT;
                    break;
                case ConversationType.PublicChatroom:
                    maxNEntries = GlobalConstants.Lengths.N_MESSAGES_LOAD_PUBLIC_CHATROOM;
                    break;
                default:
                    maxNEntries = GlobalConstants.Lengths.DEFAULT_MAX_N_ENTRIES_LOAD;
                    break;
            }
            if (nEntries == null || nEntries <= 0)
            {
                return maxNEntries;
            }
            if ((int)nEntries > maxNEntries)
                return maxNEntries;
            return (int)nEntries;
        }
        public void RemoveUserFromActiveConversations_Here(
            long userId, 
            ConversationTypeWithConversationIds[] conversationTypeWithConversationIdss)
        {
            foreach(ConversationTypeWithConversationIds c in conversationTypeWithConversationIdss)
            {
                Func<long, IActiveUsers> getActiveUsers;
                    switch (c.ConversationType)
                {
                    case ConversationType.Comments:
                        getActiveUsers = DalComments.Instance.GetIfInMemory;
                        break;
                    case ConversationType.Wall:
                        getActiveUsers= DalWalls.Instance.GetIfInMemory;
                        break;
                    default:
                        throw new NotImplementedException($"Not implemented for {Enum.GetName(typeof(ConversationType), c.ConversationType)}");
                }
                foreach (long conversationId in c.ConversationIds) {
                    getActiveUsers(conversationId)?.RemoveActiveUser(userId);
                }
            }
        }
        /*
        private string[] _GetPmUserNames(long myUserId, long otherUserId) {
            string myUserName = FrequentlyAccessedUserProfilesManager.Instance.Get(myUserId)?.Name;
            string otherUserName = FrequentlyAccessedUserProfilesManager.Instance.Get(otherUserId)?.Name;
            return new string[] { myUserName, otherUserName };
        }*/
        /*
        private Conversation FixMissingUserIdsOnPmConversationIfNecessary(Conversation conversation, long myUserId, long otherUserId)
        {
            if (conversation.UserIds.Contains(myUserId) && conversation.UserIds.Contains(otherUserId)) return conversation;
            DalConversations.Instance.ModifyConversation(conversation.ConversationId, (toModify) =>
            {
                toModify.EnsureHasUserIds(myUserId, otherUserId);
                conversation = toModify;
                return toModify;
            });
            return conversation;
        }*/
        private void SendMessageToCoreServersForUsers(
            long[] conversationUserIds, ClientMessage message)
        {
            if (conversationUserIds == null || !conversationUserIds.Any()) return;
            NodeAndAssociatedIds[] nodeAndAssociatedIdss = _NodesIdRangesUsersManager.GetNodesForIdsInRange(conversationUserIds);
            if (nodeAndAssociatedIdss == null) return;

            ParallelOperationHelper.RunInParallelNoReturn(
                   nodeAndAssociatedIdss, (nodeAndAssociatedIds) => {
                       if (nodeAndAssociatedIds.Node.Id == _MyNodeId)
                       {
                           SendMessageAsCoreServerForUsers_Here(
                               userIds: nodeAndAssociatedIds.Ids,
                               message, conversationUserIds.Take(4).ToArray()
                           );
                           return;
                       }
                       _SendMessageToCoreServerForUsers_Remote(nodeAndAssociatedIds, message);
                   }, GlobalConstants.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_CORE_SERVERS_FOR_USER);

        }
        private void SendMessageAsCoreServerForUsers_Here(long[] userIds,
            ClientMessage message,
            long[] userIdsInConversationNotAlwaysNeeded)
        {
            try
            {
                SendMessageToUsersDevices_NewThread(userIds, message);
                if (message.ConversationType.Equals(ConversationType.Pm))
                {
                    ParallelOperationHelper.RunInParallelNoReturn(
                        userIds,
                        Get_UpdateLatestMessageInConversationForUser(
                            message, userIdsInConversationNotAlwaysNeeded),
                        GlobalConstants.Threading.MAX_N_THREADS_SEND_MESSAGE_AS_CORE_SERVER_FOR_USERS
                    );
                    UpdatePmsNotificationForUsers(userIds, message);
                }
            }
            catch (Exception ex) { Logs.Default.Error(ex); }
        }
        private Action<long> Get_UpdateLatestMessageInConversationForUser(
                ClientMessage receivedMessage, long[] userIdsInConversationNotAlwaysNeeded)
        {
            return (userId) =>
            {
                try
                {
                    ConversationSnapshotsManager.Instance.UpdateLatestMessage_Here(userId, receivedMessage, userIdsInConversationNotAlwaysNeeded);
                    
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            };
        }
        private void _SendMessageToCoreServerForUsers_Remote(
            NodeAndAssociatedIds nodeAndAssociatedIds, ClientMessage receivedMessage)
        {
            try
            {
                INodeEndpoint nodeEndpoint = InterserverPort.Instance.InterserverEndpoints.GetEndpoint(nodeAndAssociatedIds.Node.Id);
                SendMessageAsCoreServerForUsersRequest message =
                    new SendMessageAsCoreServerForUsersRequest(nodeAndAssociatedIds.Ids, receivedMessage);
                string jsonString = Json.Serialize(message);
                nodeEndpoint.SendJSONString(jsonString);
            }
            catch (Exception ex) { Logs.Default.Error(ex); }
        }
        private void SendMessageToUsersDevices_NewThread(
            long[] userIds, ClientMessage message)
        {
            string jsonString = Json.Serialize(message);
            new Thread(() => _SendMessageToUsersDevices(userIds, jsonString)).Start();
        }
        private void _SendMessageToUsersDevices(long[] userIds, string receivedMessageJsonString)
        {
            NodeAndAssociatedUserIdsSessionIds[] nodeAndAssociatedUserIdsSessionIds_s = CoreUserRoutingTable.Instance.GetNodeAndAssociatedUserIdsSessionIds(userIds,
                out long[] userIdsRequiringForwardingToUsersMachines);
            ParallelOperationHelper.RunInParallelNoReturn(
                nodeAndAssociatedUserIdsSessionIds_s,
                _SendMessageToUsersDevices_SpecificNode(receivedMessageJsonString),
                GlobalConstants.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_USERS_DEVICES
            );
        }
        private Action<NodeAndAssociatedUserIdsSessionIds> _SendMessageToUsersDevices_SpecificNode(
            string receivedMessageJsonString)
        {
            return (nodeAndAssociatedUserIdsSessionIds) => {
                if (nodeAndAssociatedUserIdsSessionIds.NodeId == _MyNodeId)
                {
                    _SendMessageToUsersDevices_Here(nodeAndAssociatedUserIdsSessionIds
                        .UserIdSessionIdss.Select(u => u.UserId), receivedMessageJsonString);
                    return;
                }
                _SendMessageToUsersDevices_ToMachineWithUsersDevices(nodeAndAssociatedUserIdsSessionIds, receivedMessageJsonString);
            };
        }
        private void _SendMessageToUsersDevices_Here(IEnumerable<long> userIds, string receivedMessageJsonString)
        {
            IClientEndpoint[] clientEndpoints = CoreUserRoutingTable.Instance.GetEndpointsForUserIds(userIds, out long[] ignore);
            ParallelOperationHelper.RunInParallelNoReturn(
                clientEndpoints,
                _Get_SendMessageToUsersDevice(receivedMessageJsonString),
                GlobalConstants.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_USERS_DEVICES_HERE
            );
        }
        private void _SendMessageToUsersDevices_ToMachineWithUsersDevices(
            NodeAndAssociatedUserIdsSessionIds nodeAndAssociateUserIdsSessionIds, string receivedMessageJsonString)
        {
            try
            {
                INodeEndpoint nodeEndpoint = InterserverPort.Instance.InterserverEndpoints.GetEndpoint(
                    nodeAndAssociateUserIdsSessionIds.NodeId);
                SendMessageToUsersDevicesRequest request = new SendMessageToUsersDevicesRequest(
                    nodeAndAssociateUserIdsSessionIds.UserIdSessionIdss.Select(u => u.UserId).ToArray(), receivedMessageJsonString);
                string jsonString = Json.Serialize(request);
                nodeEndpoint.SendJSONString(jsonString);
            }
            catch (Exception ex) { Logs.Default.Error(ex); }
        }
        private Action<IClientEndpoint> _Get_SendMessageToUsersDevice(string receivedMessageJsonString)
        {
            return (clientEndpoint) => {
                clientEndpoint.SendJSONString(receivedMessageJsonString);
            };
        }
        private Administrator[]? GetAdministrators_Here(long conversationId,
            long myUserId, out AdministratorsFailedReason? failedReason)
        {
            ChatRoom chatRoom = ChatRooms.Instance.GetIfExists(conversationId);
            if (chatRoom == null)
            {
                failedReason = AdministratorsFailedReason.RoomDoesntExist;
                return null;
            }
            return chatRoom.GetAdministrators(myUserId, out failedReason);
        }
        private void UpdatePmsNotificationForUsers(long[] userIds, ClientMessage message) {
            long notSenderUserId = userIds.Where(u => u != message.UserId).FirstOrDefault();
            if(notSenderUserId == 0) { return; }
            NotificationsCore.UserNotificationsMesh.Instance.SetHasAt((long)notSenderUserId, NotificationType.Pms, message.SentAt);
        }
        public void SetSeenMessage_Here(long myUserId, long conversationId, long messageId) {
            ConversationSnapshotsManager.Instance.SetSeenMessage_Here(myUserId, conversationId, messageId);
            UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                new SetSeenMessage(myUserId, conversationId, messageId), myUserId
            );
        }
    }
}
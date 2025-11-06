using Core.Exceptions;
using Shutdown;
using InterserverComs;
using NodeAssignedIdRanges;
using Core.Timing;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using Core.Messages.Responses;
using Users;
using Logging;
using UserRoutedMessages;
using HashTags.Messages;
using HashTags;
using Core.Strings;
using Core.Threading;
using HashTags.Enums;
using Initialization.Exceptions;
namespace Chat
{
    public partial class ChatManager
    {
        private static ChatManager _Instance;
        public static ChatManager Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(ChatManager));
            _Instance = new ChatManager();
            return _Instance;
        }
        public static ChatManager Instance { 
            get { 
                if (_Instance == null) throw new NotInitializedException(nameof(ChatManager));
                return _Instance;
            } 
        }
        private long _MyNodeId;
        private NodesIdRangesForIdTypeManager _NodesIdRangesUsersManager;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private MessageIdSource _MessageIdSource;
        private ChatManager() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _NodesIdRangesUsersManager = NodesIdRangesManager.Instance.ForIdType(Configurations.IdTypes.USER);
            _MessageIdSource = MessageIdSource.Instance;
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.ChatsManager);
        }
        #region Methods
        #region Public
        public bool GetConversationSnapshots(long myUserId,
            long? idFromInclusive, long? idToExclusive, int? nEntries, 
            out ConversationSnapshot[] messages, out MessageReaction[] reactions,
            out MessageUserMultimediaItem[] messageUserMultimediaItems, 
            out ChatFailedReason failedReason)
        {

            bool successful = true;
            ConversationSnapshot[] messagesInternal = null;
            MessageReaction[] reactionsInternal = null;
            MessageUserMultimediaItem[] messageUserMultimediaItemsInternal = null;
            ChatFailedReason failedReasonInternal = ChatFailedReason.None;
            OperationRedirectHelper.OperationRedirectedToNode<GetConversationSnapshotsRequest,
                GetConversationSnapshotsResponse>(
                UserIdToNodeId.Instance.GetNodeIdFromIdentifier(myUserId),
                () =>
                {
                    successful = GetConversationSnapshots_Here(myUserId, idFromInclusive, idToExclusive,
                        nEntries,  out messagesInternal, out reactionsInternal,
                        out messageUserMultimediaItemsInternal);
                },
                () => new GetConversationSnapshotsRequest(myUserId, idFromInclusive, idToExclusive, nEntries),
                (response) =>
                {
                    if (!response.Successful)
                    {
                        failedReasonInternal = response.FailedReason;
                        successful = false;
                        return;
                    }
                    messagesInternal = response.Entries;
                },
                _CancellationTokenSourceDisposed.Token
            );
            failedReason = failedReasonInternal;
            messages = messagesInternal;
            reactions = reactionsInternal;
            messageUserMultimediaItems = messageUserMultimediaItemsInternal;
            return successful;
        }
        public bool GetPmConversation(
            long myUserId,
            long otherUserId,
            out long conversationId,
            out ChatFailedReason failedReason)
        {
            bool successful = true;
            long conversationIdInternal = 0;
            ChatFailedReason failedReasonInternal = ChatFailedReason.None;
            OperationRedirectHelper.OperationRedirectedToNode<
                GetPmConversationInterserverRequest,
                GetConversationIdResponse>(
                UserIdToNodeId.Instance.GetNodeIdFromIdentifier(myUserId > otherUserId ? myUserId : otherUserId),
                () =>
                {
                    conversationIdInternal = GetPmConversation_Here(
                        myUserId,
                        otherUserId,
                        out failedReasonInternal);
                },
                () => new GetPmConversationInterserverRequest(
                    myUserId,
                    otherUserId),
                (response) =>
                {
                    if (!response.Successful)
                    {
                        successful = false;
                    }
                    conversationIdInternal = response.ConversationId;
                    failedReasonInternal = response.FailedReason;
                },
                _CancellationTokenSourceDisposed.Token
            );
            conversationId = conversationIdInternal;
            failedReason = failedReasonInternal;
            return successful;
        }
        public bool GetWallConversation(
            long myUserId,
            long userId,
            out long conversationId,
            out ChatFailedReason failedReason,
            out int nodeId)
        {
            long conversationIdInternal = -1;
            ChatFailedReason failedReasonInternal = ChatFailedReason.ServerError;
            bool successful = true;
            nodeId = UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userId);
            OperationRedirectHelper.OperationRedirectedToNode<
                GetWallConversationRequest,
                GetConversationIdResponse>(
                nodeId,
                () =>
                {
                    conversationIdInternal = GetWallConversation_Here(myUserId, userId,
                        out failedReasonInternal);
                },
                () => new GetWallConversationRequest(
                    myUserId,
                    userId),
                (response) =>
                {
                    if (!response.Successful)
                    {
                        successful = false;
                    }
                    conversationIdInternal = response.ConversationId;
                    failedReasonInternal = response.FailedReason;
                },
                ShutdownManager.Instance.CancellationToken
            );
            conversationId = (long)conversationIdInternal;
            failedReason = failedReasonInternal;
            return successful;
        }
        public bool GetWallCommentsConversation(
            long myUserId,
            long wallConversationId, 
            long wallMessageId,
            out long conversationId,
            out ChatFailedReason failedReason, out int nodeId)
        {
            long conversationIdInternal = 0;
            ChatFailedReason failedReasonInternal = ChatFailedReason.ServerError;
            bool successful = true;
            nodeId =
                ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(wallConversationId);
            OperationRedirectHelper.OperationRedirectedToNode<
                GetWallCommentsConversationRequest,
                GetConversationIdResponse>(
                nodeId,
                () =>
                {
                     successful = GetWallCommentsConversation_Here(
                        myUserId,
                        wallConversationId,
                        wallMessageId, 
                        out conversationIdInternal, 
                        out failedReasonInternal);
                },
                () => new GetWallCommentsConversationRequest(
                    myUserId,
                    wallConversationId,
                    wallMessageId),
                (response) =>
                {
                    if (!response.Successful)
                    {
                        successful = false;
                    }
                    conversationIdInternal = response.ConversationId;
                },
                ShutdownManager.Instance.CancellationToken
            );
            conversationId = conversationIdInternal;
            failedReason = failedReasonInternal;
            return successful;
        }
        public bool SendMessage(
            SendMessageRequest message, out ChatFailedReason sendMessageFailedReason, 
            out ClientMessage replyMessage) {
            bool successful = true;
            message.Id = _MessageIdSource.NextId();
            message.SentAt = TimeHelper.MillisecondsNow;
            message.ReplyMessage = null;
            ClientMessage replyMessageInternal = null;
           ChatFailedReason sendMessageFailedReasonInternal = ChatFailedReason.None;
            OperationRedirectHelper.OperationRedirectedToNode<SendMessageRequest,
                SendMessageResponse>(
                ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(message.ConversationId),
                () =>
                {
                    successful = SendMessage_Here(message, out sendMessageFailedReasonInternal, out replyMessageInternal);
                },
                () => message,
                (sendMessageResponse) =>
                {
                    if (!sendMessageResponse.Successful)
                    {
                        sendMessageFailedReasonInternal = sendMessageResponse.FailedReason;
                        replyMessageInternal = sendMessageResponse.ReplyMessage;
                        successful = false;
                    }
                },
                _CancellationTokenSourceDisposed.Token
            );
            sendMessageFailedReason = sendMessageFailedReasonInternal;
            replyMessage = replyMessageInternal;
            return successful;
        }

        public FetchConversationIndividualMessagesResult[] FetchIndividualMessages(long myUserId, ConversationAndMessageIds[] conversationAndMessageIds)
        {
            var nodeIdAndConversationAndMessageIdss = conversationAndMessageIds.Select(c => new {
                nodeId = ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(c.ConversationId),
                conversationAndMessageIds = c
            }).GroupBy(o => o.nodeId);
            List < FetchConversationIndividualMessagesResult > results = new List<FetchConversationIndividualMessagesResult>(conversationAndMessageIds.Length);
            ParallelOperationHelper.RunInParallelNoReturn(nodeIdAndConversationAndMessageIdss, (n) =>
            {
                try
                {
                    OperationRedirectHelper.OperationRedirectedToNode<FetchIndividualMessagesRequest,
                        FetchIndividualMessagesResponse>(
                        n.First().nodeId,
                        () =>
                        {
                            results.AddRange(FetchIndividualMessages_Here(myUserId, n.Select(o => o.conversationAndMessageIds)));
                        },
                        () => new FetchIndividualMessagesRequest(myUserId, n.Select(o => o.conversationAndMessageIds).ToArray()),
                        (response) =>
                        {
                            results.AddRange(response.Results);
                        },
                        _CancellationTokenSourceDisposed.Token
                    );
                }
                catch (Exception ex) {
                    Logs.Default.Error(ex);
                    results.AddRange(n.Select(m=>new FetchConversationIndividualMessagesResult(m.conversationAndMessageIds.ConversationId, ChatFailedReason.ServerError )));
                }
            },
            5);
            return results.ToArray();
        }
        public bool LoadMessagesHistory(long myUserId, long conversationId, 
            ConversationType conversationType, long? idFromInclusive, long? idToExclusive, int? nEntries, 
            out ClientMessage[] messages, out MessageReaction[] reactions, 
            out MessageUserMultimediaItem[] messageUserMultimediaItems, out ChatFailedReason failedReason, 
            MessageChildConversationOptions messageChildConversationOptions)
        {
            bool successful = true;
            ClientMessage[] messagesInternal = null;
            MessageReaction[] reactionsInternal = null;
            MessageUserMultimediaItem[] messageUserMultimediaItemsInternal = null;
            ChatFailedReason failedReasonInternal = ChatFailedReason.None;
            OperationRedirectHelper.OperationRedirectedToNode<LoadMessagesHistoryRequest,
                LoadMessagesHistoryResponse>(
                ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                () =>
                {
                    successful = LoadMessagesHistory_Here(myUserId, conversationId,
                        conversationType, idFromInclusive, idToExclusive, nEntries,
                        out messagesInternal, out reactionsInternal, 
                        out messageUserMultimediaItemsInternal, out failedReasonInternal,
                        messageChildConversationOptions);
                },
                () => new LoadMessagesHistoryRequest(myUserId, conversationId, idFromInclusive, idToExclusive, nEntries),
                (response) =>
                {
                    if (!response.Successful)
                    {
                        failedReasonInternal = response.FailedReason;
                        successful = false;
                        return;
                    }
                    messagesInternal = response.Entries;
                },
                _CancellationTokenSourceDisposed.Token
            );
            failedReason = failedReasonInternal;
            messages = messagesInternal;
            reactions = reactionsInternal;
            messageUserMultimediaItems = messageUserMultimediaItemsInternal;
            return successful;
        }
        public long[] DeleteMessages(long myUserId, long conversationId, 
            ConversationType conversationType, long[] messageIds, bool canDeleteAnyMessage = false)
        {
            long[] deletedIds = null;
            OperationRedirectHelper.OperationRedirectedToNode<
                DeleteMessagesRequest, DeleteMessagesResponse>(
                ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                () =>
                {
                    deletedIds = DeleteMessages_Here(myUserId, conversationId, conversationType, messageIds, canDeleteAnyMessage);
                },
                () => new DeleteMessagesRequest(myUserId, conversationId, messageIds, canDeleteAnyMessage),
                (response) =>
                {
                    deletedIds = response.DeletedIds;
                },
                ShutdownManager.Instance.CancellationToken
            );
            return deletedIds;
        }
        public bool ModifyMessage(long conversationId, ConversationType conversationType, ClientMessage message)
        {
            bool successful = true;
            OperationRedirectHelper.OperationRedirectedToNode<
                ModifyMessage, SuccessTicketedResponse>(
                ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                () =>
                {
                    ModifyMessage_Here(conversationId, conversationType, message);
                },
                () => new ModifyMessage(conversationId, message),
                (response) =>
                {
                    successful = response.Success;
                },
                ShutdownManager.Instance.CancellationToken
            );
            return successful;
        }
        public void ReactToMessage(long conversationId, ConversationType conversationType, MessageReaction reaction)
        {
            OperationRedirectHelper.OperationRedirectedToNode(
                ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                () =>
                {
                    ReactToMessage_Here(conversationId, conversationType, reaction);
                },
                () => new ReactToMessage(conversationId, conversationType, reaction)
            );
        }
        public void UnreactToMessage(long conversationId, ConversationType conversationType, MessageReaction reaction)
        {
            OperationRedirectHelper.OperationRedirectedToNode(
                ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                () =>
                {
                    UnreactToMessage_Here(conversationId, conversationType, reaction);
                },
                () => new UnreactToMessage(conversationId, reaction)
            );
        }
        public void RemoveUserFromActiveConversations(long userId, int nodeId,
            ConversationTypeWithConversationIds[] conversationTypeWithConversationIdss)
        {
            OperationRedirectHelper.OperationRedirectedToNode<RemoveUserFromActiveConversationsInternalRequest>(
                nodeId,
                () => RemoveUserFromActiveConversations_Here(userId, conversationTypeWithConversationIdss),
                () => new RemoveUserFromActiveConversationsInternalRequest(conversationTypeWithConversationIdss, userId));
        }
        /*
        public MessageWithTags SearchMessages() { 
            
        }*/
        public ConversationWithTags[] SearchRooms(string str)
        {
            string[] tags = StringHelper.MultipleSplit(new char[] { ' ', ',' }, str).Take(5).ToArray();
            Dictionary<long, List<Tuple<string, bool>>> mapRoomIdToTagAndIsExactMatches 
                = new Dictionary<long, List<Tuple<string, bool>>>();
            foreach (string tag in tags)
            {
                HashTagsMesh.Instance.SearchTags(str, HashTagScopeTypes.ChatRoom, allowPartialMatches: true,
                   20, out ScopeIds[]? exactMatches, out TagWithScopeIds[]? partialMatches);
                if (exactMatches != null)
                {
                    foreach (var exactMatch in exactMatches)
                    {
                        var tagAndIsExactMatch = new Tuple<string, bool>(tag, true);
                        if (mapRoomIdToTagAndIsExactMatches.TryGetValue(exactMatch.ScopeId, out List<Tuple<string, bool>> tagAndIsExactMatches))
                        {
                            tagAndIsExactMatches.Add(tagAndIsExactMatch);
                        }
                        else {
                            mapRoomIdToTagAndIsExactMatches.Add(exactMatch.ScopeId, new List<Tuple<string, bool>> { tagAndIsExactMatch });
                        }
                    }
                }
                if (partialMatches != null)
                {
                    foreach (var partialMatch in partialMatches)
                    {
                        var tagAndIsExactMatch = new Tuple<string, bool>(partialMatch.Tag, false);
                        if (mapRoomIdToTagAndIsExactMatches.TryGetValue(partialMatch.ScopeId,
                            out List<Tuple<string, bool>> tagAndIsExactMatches))
                        {
                            tagAndIsExactMatches.Add(tagAndIsExactMatch);
                        }
                        else
                        {
                            mapRoomIdToTagAndIsExactMatches.Add(partialMatch.ScopeId,
                                new List<Tuple<string, bool>> { tagAndIsExactMatch });
                        }
                    }
                }
            }
            return mapRoomIdToTagAndIsExactMatches.Select(p =>
                new ConversationWithTags(p.Key, p.Value
                    .Where(v => v.Item2)
                    .Select(v => v.Item1)
                    .ToArray(),
                    p.Value
                    .Where(v => !v.Item2)
                    .Select(v => v.Item1)
                    .ToArray()))
                .ToArray();
        }
        public void SetSeenMessage(long myUserId, long conversationId, long messageId)
        {
            OperationRedirectHelper.OperationRedirectedToNode<SetSeenMessage>(
                UserIdToNodeId.Instance.GetNodeIdFromIdentifier(myUserId),
                () => SetSeenMessage_Here(myUserId, conversationId, messageId),
                () => new SetSeenMessage(myUserId, conversationId, messageId)
            );
        }
        #endregion Public
        #region Private
        private void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
        }

        #endregion Private
#endregion Methods
    }
}
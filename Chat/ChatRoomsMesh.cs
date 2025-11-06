using Core.DAL;
using Core.Exceptions;
using Shutdown;
using InterserverComs;
using NodeAssignedIdRanges;
using Core.Threading;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Core.Messages.Responses;
using Logging;
using Users;
using Chat.Messages.Client.Messages;
using UserRoutedMessages;
using Initialization.Exceptions;
namespace Chat
{
    public partial class ChatRoomsMesh
    {
        private static ChatRoomsMesh _Instance;
        public static ChatRoomsMesh Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(ChatRoomsMesh));
            _Instance = new ChatRoomsMesh();
            return _Instance;
        }
        public static ChatRoomsMesh Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(ChatRoomsMesh));
                return _Instance;
            }
        }
        private long _MyNodeId;
        private NodesIdRangesForIdTypeManager _NodesIdRangesUsersManager;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private DalUserRooms _DalUserRooms;
        private ChatRoomsMesh()
        {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _NodesIdRangesUsersManager = NodesIdRangesManager.Instance.ForIdType(Configurations.IdTypes.USER);
            _DalUserRooms = DalUserRooms.Instance;
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.ChatsManager);
        }
        #region Methods
        #region Public
        public bool GetUserPinnedRooms(long myUserId, out string entriesSerialized)
        {
            return GetUserRooms(myUserId, UserRoomsOperation.Pinned, out entriesSerialized);
        }
        public bool GetUserRecentRooms(long myUserId, out string entriesSerialized)
        {
            return GetUserRooms(myUserId, UserRoomsOperation.Recent, out entriesSerialized);
        }
        public bool GetUserMyRooms(long myUserId, out string entriesSerialized)
        {
            return GetUserRooms(myUserId, UserRoomsOperation.Mine, out entriesSerialized);
        }
        public bool GetUserPopularRooms(long myUserId, out string entriesSerialized)
        {
            return GetUserRooms(myUserId, UserRoomsOperation.Popular, out entriesSerialized);
        }
        public bool GetUserRooms(long myUserId, UserRoomsOperation operation, out string entriesSerialized)
        {
            bool success = true;
            string entriesSerializedInternal = null;
            try
            {
                int nodeId = operation.Equals(UserRoomsOperation.Popular)
                    ?
#if DEBUG
                    Configurations.Nodes.ECHAT_POPULAR_ROOMS_MANAGER_DEBUG
#else
                    Configurations.Nodes.ECHAT_POPULAR_ROOMS_MANAGER
#endif
                    : _NodesIdRangesUsersManager.GetNodeIdForIdInRange(myUserId);
                OperationRedirectHelper.OperationRedirectedToNode<
                    GetUserRoomsRequest, GetUserRoomsResponse>(
                    nodeId,
                    () =>
                    {
                        entriesSerializedInternal = GetRooms_Here(myUserId, operation);
                    },
                    () => new GetUserRoomsRequest(myUserId, operation),
                    (response) =>
                    {
                        success = response.Successful;
                        entriesSerializedInternal = response.EntriesSerialized;
                    },
                    _CancellationTokenSourceDisposed.Token
                );
                entriesSerialized = entriesSerializedInternal;
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                entriesSerialized = null;
                return false;
            }
        }
        public bool ModifyUserRooms(long myUserId, long conversationId, bool addElseRemove, params UserRoomsOperation[] operation)
        {
            bool success = true;
            try
            {
                int nodeId = _NodesIdRangesUsersManager.GetNodeIdForIdInRange(myUserId);
                OperationRedirectHelper.OperationRedirectedToNode<
                    ModifyUserRoomsRequest, SuccessTicketedResponse>(
                    nodeId,
                    () =>
                    {
                        ModifyUserRooms_Here(myUserId, conversationId, addElseRemove, operation);
                    },
                    () => new ModifyUserRoomsRequest(myUserId, conversationId, addElseRemove, operation),
                    (response) =>
                    {
                        success = response.Success;
                    },
                    _CancellationTokenSourceDisposed.Token
                );
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
        public List<RoomActivity> GetMostActiveRoomsFromAllRoomServerNodes(int nMostActive)
        {
            List<RoomActivity> allMostActiveRooms = new List<RoomActivity>();
            ParallelOperationHelper.RunInParallelNoReturn(
                Configurations.Nodes.Instance.GetNodeIdsAssociatedWithIdType(Configurations.IdTypes.CONVERSATION)
                , (nodeId) =>
                {
                    try
                    {
                        RoomActivity[] mostActiveRooms = null;
                        OperationRedirectHelper.OperationRedirectedToNode<GetMostActiveRoomsRequest,
                            GetMostActiveRoomsResponse>(nodeId,
                            () =>
                            {
                                mostActiveRooms = GetMostActiveRooms_Here(nMostActive);
                            },
                            () => new GetMostActiveRoomsRequest(nMostActive),
                            (response) =>
                            {
                                mostActiveRooms = response.MostActiveRooms;
                            },
                            _CancellationTokenSourceDisposed.Token
                        );
                        lock (allMostActiveRooms)
                        {
                            allMostActiveRooms.AddRange(mostActiveRooms);
                        }
                    }
                    catch (Exception ex) {
                        Logs.Default.Error(ex);
                    }
                }, Configurations.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_CORE_SERVERS_FOR_USER);
            return allMostActiveRooms;
        }
        public string GetMostActiveRoomsFromManager()
        {
            string mostActiveRooms = null;
            OperationRedirectHelper.OperationRedirectedToNode<GetMostActiveRoomsFromManagerRequest,
                GetMostActiveRoomsFromManagerResponse>(
                Configurations.Nodes.ECHAT_MOST_ACTIVE_ROOMS_MANAGER,
                () =>
                {
                    throw new Exception("Something went wrong. This should not be getting called on the manager node");
                },
                () => new GetMostActiveRoomsFromManagerRequest(),
                (response) =>
                {
                    mostActiveRooms = response.MostActiveRooms;
                },
                _CancellationTokenSourceDisposed.Token
            );
            return mostActiveRooms;
        }
        public bool GetRoomSummarys(long[] conversationIds, out RoomSummary[] roomSummarys)
        {
            try
            {
                List<RoomSummary> roomSummarysInternal = new List<RoomSummary>();
                ParallelOperationHelper.RunInParallelNoReturn(
                    ConversationIdToNodeId.Instance.GetNodeIds(conversationIds),
                    (nodeIdAndAssociatedIds) =>
                    {
                        OperationRedirectHelper.OperationRedirectedToNode<GetRoomSummarysRequest, GetRoomSummarysResponse>(
                            nodeIdAndAssociatedIds.NodeId,
                            () =>
                            {
                                RoomSummary[] roomSummarys = GetChatRoomSummarys_Here(nodeIdAndAssociatedIds.Ids);
                                if (roomSummarys != null)
                                    roomSummarysInternal.AddRange(roomSummarys);
                            },
                            () => new GetRoomSummarysRequest(nodeIdAndAssociatedIds.Ids),
                            (response) =>
                            {
                                if (!response.Successful) return;
                                lock (roomSummarysInternal)
                                {
                                    roomSummarysInternal.AddRange(response.Summarys);
                                }
                            },
                            ShutdownManager.Instance.CancellationToken);
                    }, Configurations.Threading.MAX_N_THREADS_GET_ROOM_SNAPSHOTS);
                roomSummarys = roomSummarysInternal.ToArray();
                return true;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                roomSummarys = null;
                return false;
            }
        }
        public InviteFailedReason? RoomInvite(
            long conversationId, long otherUserId, long myUserId)
        {
            try
            {
                InviteFailedReason? failedReason = null;
                OperationRedirectHelper.OperationRedirectedToNode<
                    RoomInviteRequest, RoomInviteResponse>(
                        ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                        () =>
                        {
                            RoomInvite_Here(conversationId, otherUserId, myUserId);
                        },
                        () => new RoomInviteRequest(conversationId, otherUserId, myUserId),
                        (response) =>
                        {
                            failedReason = response.FailedReason;
                        },
                        ShutdownManager.Instance.CancellationToken);
                if (failedReason == null) {

                    UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                            new AddRoomInvite(conversationId, otherUserId, myUserId),
                            new long[] { myUserId, otherUserId}
                        );
                }
                return failedReason;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return InviteFailedReason.ServerError;
            }
        }

        public JoinFailedReason? AcceptRoomInvite(
            long conversationId, long myUserId)
        {
            try
            {
                JoinFailedReason? failedReason = null;
                OperationRedirectHelper.OperationRedirectedToNode<
                    AcceptRoomInviteRequest, AcceptRoomInviteResponse>(
                        ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(myUserId),
                        () =>
                        {
                            AcceptRoomInvite_Here(conversationId, myUserId);
                        },
                        () => new AcceptRoomInviteRequest(conversationId, myUserId),
                        (response) =>
                        {
                            failedReason = response.FailedReason;
                        },
                        ShutdownManager.Instance.CancellationToken);
                if (failedReason == null)
                {
                    try
                    {
                        if (RemoveReceivedInvite(conversationId, myUserId, out long[] userIdsInvitingRemoved)
                            &&userIdsInvitingRemoved != null)
                        {
                            foreach (long userIdInvitingRemoved in userIdsInvitingRemoved)
                            {
                                try
                                {
                                    RemoveSentInvite(conversationId, myUserId, userIdInvitingRemoved);
                                }
                                catch (Exception ex)
                                {
                                    Logs.Default.Error(ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.Default.Error(ex);
                    }
                }
                return failedReason;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return JoinFailedReason.ServerError;
            }
        }

        public bool RejectRoomInvite(
            long conversationId, long myUserId)
        {
            try
            {
                bool success = RemoveReceivedInvite(conversationId, myUserId, out long[] userIdsInvitingRemoved);
                try
                {
                    RemoveInviteFromRoom(conversationId, myUserId);
                }
                catch(Exception ex){
                    Logs.Default.Error(ex);
                }
                if (success)
                {
                    if (userIdsInvitingRemoved != null)
                    {
                        foreach (long userIdInvitingRemoved in userIdsInvitingRemoved)
                        {
                            try
                            {
                                RemoveSentInvite(conversationId, myUserId, userIdInvitingRemoved);
                            }
                            catch (Exception ex)
                            {
                                Logs.Default.Error(ex);
                            }
                        }
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }

        public bool CancelRoomInvite(
            long conversationId, long myUserId, long otherUserId)
        {
            try
            {
                    RemoveInviteFromRoom(conversationId, otherUserId, myUserId);
                try
                {
                    RemoveReceivedInvite(conversationId, otherUserId, out long[] ignore, myUserId);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                    return false;
                }
                try
                {
                    RemoveSentInvite(conversationId, otherUserId, myUserId);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }

        public RemoveRoomUserFailedReason? LeaveRoom(
            long conversationId, long myUserId, bool allowRemoveOnlyFullAdmin)
        {
            RemoveRoomUserFailedReason? failedReason = RemoveUserFromRoom(conversationId, myUserId, allowRemoveOnlyFullAdmin);
            try
            {
                ModifyUserRooms(myUserId, conversationId, addElseRemove: false,
                    UserRoomsOperation.Joined, UserRoomsOperation.Mine,
                    UserRoomsOperation.Pinned);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return RemoveRoomUserFailedReason.ServerError;
            }
            return failedReason;
        }

        public bool GetMyReceivedInvites(long myUserId, out Invites? invites)
        {
            try
            {
                bool success = true;
                Invites? invitesInternal= null;
                OperationRedirectHelper.OperationRedirectedToNode<
                    GetMyReceivedInvitesRequest, GetMyReceivedInvitesResponse>(
                        UserIdToNodeId.Instance.GetNodeIdFromIdentifier(myUserId),
                        () =>
                        {
                            invitesInternal = GetMyReceivedInvites_Here(myUserId);
                        },
                        () => new GetMyReceivedInvitesRequest(myUserId),
                        (response) =>
                        {
                            success = response.Success;
                            invitesInternal = response.Invites;
                        },
                        ShutdownManager.Instance.CancellationToken);
                invites = invitesInternal;
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                invites = null;
                return false;
            }
        }
        public bool GetMySentInvites(long myUserId, out Invites? invites)
        {
            try
            {
                bool success = true;
                Invites? invitesInternal = null;
                OperationRedirectHelper.OperationRedirectedToNode<
                    GetMySentInvitesRequest, GetMySentInvitesResponse>(
                        UserIdToNodeId.Instance.GetNodeIdFromIdentifier(myUserId),
                        () =>
                        {
                            invitesInternal = GetMySentInvites_Here(myUserId);
                        },
                        () => new GetMySentInvitesRequest(myUserId),
                        (response) =>
                        {
                            success = response.Success;
                            invitesInternal = response.Invites;
                        },
                        ShutdownManager.Instance.CancellationToken);
                invites = invitesInternal;
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                invites = null;
                return false;
            }
        }
        #endregion Public
        #region Private
        private bool RemoveInviteFromRoom(long conversationId, long userIdBeingInvited, long? userIdInviting = null) {
            try
            {
                bool success = true;
                OperationRedirectHelper.OperationRedirectedToNode<
                    RemoveInviteFromRoomRequest, RemoveInviteFromRoomResponse>(
                        ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                        () =>
                        {
                            RemoveInviteFromRoom_Here(conversationId, userIdBeingInvited, userIdInviting);
                        },
                        () => new RemoveInviteFromRoomRequest(conversationId, userIdBeingInvited, userIdInviting),
                        (response) =>
                        {
                            success = response.Success;
                        },
                        ShutdownManager.Instance.CancellationToken
                    );
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
        private bool AddReceivedInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            try
            {
                bool success = true;
                OperationRedirectHelper.OperationRedirectedToNode<
                    AddReceivedInviteRequest, AddReceivedInviteResponse>(
                        UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userIdBeingInvited),
                        () =>
                        {
                            AddReceivedInvite_Here(conversationId, userIdBeingInvited, userIdInviting);
                        },
                        () => new AddReceivedInviteRequest(conversationId, userIdBeingInvited, userIdInviting),
                        (response) =>
                        {
                            success = response.Success;
                        },
                        ShutdownManager.Instance.CancellationToken
                    );
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
        private bool AddSentInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            try
            {
                bool success = true;
                OperationRedirectHelper.OperationRedirectedToNode<
                    AddSentInviteRequest, AddSentInviteResponse>(
                        UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userIdInviting),
                        () =>
                        {
                            AddSentInvite_Here(conversationId, userIdBeingInvited, userIdInviting);
                        },
                        () => new AddSentInviteRequest(conversationId, userIdBeingInvited, userIdInviting),
                        (response) =>
                        {
                            success = response.Success;
                        },
                        ShutdownManager.Instance.CancellationToken
                    );
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
        private bool RemoveReceivedInvite(long conversationId,
            long userIdBeingInvited, out long[] userIdsInvitingRemoved, long? userIdInviting = null)
        {
            try
            {
                bool success = true;
                long[] userIdsInvitingRemovedInternal = null;
                OperationRedirectHelper.OperationRedirectedToNode<
                    RemoveReceivedInviteRequest, RemoveReceivedInviteResponse>(
                        UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userIdBeingInvited),
                        () =>
                        {
                            RemoveReceivedInvite_Here(conversationId, userIdBeingInvited,
                                userIdInviting, out userIdsInvitingRemovedInternal);
                        },
                        () => new RemoveReceivedInviteRequest(conversationId, userIdBeingInvited, userIdInviting),
                        (response) =>
                        {
                            success = response.Success;
                            userIdsInvitingRemovedInternal = response.UserIdsInvitingRemoved;
                        },
                        ShutdownManager.Instance.CancellationToken
                    );
                userIdsInvitingRemoved = userIdsInvitingRemovedInternal;
                if (userIdsInvitingRemoved!=null)
                {
                    foreach (long userIdInvitingRemoved in userIdsInvitingRemoved)
                    {
                        UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                            new RemoveRoomInvite(conversationId, userIdBeingInvited, userIdInvitingRemoved),
                            userIdBeingInvited, userIdInvitingRemoved
                        );
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                userIdsInvitingRemoved = null;
                return false;
            }
        }
        private bool RemoveSentInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            try
            {
                bool success = true;
                OperationRedirectHelper.OperationRedirectedToNode<
                    RemoveSentInviteRequest, RemoveSentInviteResponse>(
                        UserIdToNodeId.Instance.GetNodeIdFromIdentifier(userIdInviting),
                        () =>
                        {
                            RemoveSentInvite_Here(conversationId, userIdBeingInvited, userIdInviting);
                        },
                        () => new RemoveSentInviteRequest(conversationId, userIdBeingInvited, userIdInviting),
                        (response) =>
                        {
                            success = response.Success;
                        },
                        ShutdownManager.Instance.CancellationToken
                    );
                if (success)
                {
                    UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(
                        new RemoveRoomInvite(conversationId, userIdBeingInvited, userIdInviting),
                        userIdInviting, userIdBeingInvited
                    );
                }
                return success;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
        private RemoveRoomUserFailedReason? RemoveUserFromRoom(long conversationId, long userId, bool allowRemoveOnlyFullAdmin)
        {
            try
            {
                RemoveRoomUserFailedReason? failedReason = null;
                OperationRedirectHelper.OperationRedirectedToNode<
                    RemoveUserFromRoomRequest, RemoveUserFromRoomResponse>(
                        ConversationIdToNodeId.Instance.GetNodeIdFromIdentifier(conversationId),
                        () =>
                        {
                            failedReason = RemoveUserFromRoom_Here(conversationId, userId, allowRemoveOnlyFullAdmin);
                        },
                        () => new RemoveUserFromRoomRequest(conversationId, userId),
                        (response) =>
                        {
                            failedReason = response.FailedReason;
                        },
                        ShutdownManager.Instance.CancellationToken
                    );
                return failedReason;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return RemoveRoomUserFailedReason.ServerError;
            }
        }
        private void Dispose()
        {
            _CancellationTokenSourceDisposed.Cancel();
        }

        #endregion Private
        #endregion Methods
    }
}
using Core.Threading;
using Core.Exceptions;
using Logging;
using Shutdown;
using Users.Messages.Interserver;
using Users.Messages.Client;
using JSON;
using Core.DTOs;
using Users.Enums;
using Core.Messages.Responses;
using Users.DAL;
using Authentication.DAL;
using UserRouting;
using InterserverComs;
using Nodes;
using Core.Messages.Messages;
using NodeAssignedIdRanges;
using Users.Users;
using UsersEnums;
using Core.Ids;
using UserRoutedMessages;
using Initialization.Exceptions;

namespace Users
{
    public partial class UsersMesh : IAuthenticationRelatedUserInfoSource
    {
        private static UsersMesh _Instance;
        public static UsersMesh Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(UsersMesh));
            _Instance = new UsersMesh();
            return _Instance;
        }
        public static UsersMesh Instance { 
            get { 
                if (_Instance == null) throw new NotInitializedException(nameof(UsersMesh));
                return _Instance;
            } 
        }
        private long _MyNodeId;
        private NodesIdRangesForIdTypeManager _NodeidRangsForUserManager;
        private FixedNodeIdentifierToNodeId _UsernameSearchIdentifierToNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private UsersMesh() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _UsernameSearchIdentifierToNodeId = new FixedNodeIdentifierToNodeId(
#if DEBUG
                Configurations.Nodes.ECHAT_USERNAME_SEARCH_DEBUG
#else
                Configurations.Nodes.ECHAT_USERNAME_SEARCH
#endif
            );
            _NodeidRangsForUserManager = NodesIdRangesManager.Instance.ForIdType(Configurations.IdTypes.USER);
            DalAssociateRequests.Initialize();
            DalAssociates.Initialize();
            DalUserProfiles.Initialize();
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.AssociatesManager);
        }
        #region Methods
        #region Public
        public void GetAllAssociateEntries(
            long myUserId, 
            out UserProfileSummary[] associates, 
            out AssociateRequestUserProfileSummarys receivedRequests, 
            out AssociateRequestUserProfileSummarys sentRequests)
        {
            
            UserProfileSummary[] associatesInternal = null;
            AssociateRequestUserProfileSummarys receivedRequestsInternal = null;
            AssociateRequestUserProfileSummarys sentRequestsInternal = null;
            OperationRedirectHelper.OperationRedirectedToNode<
                            GetAllAssociateEntriesRequest, GetAllAssociateEntriesResponse>(
                            UserIdToNodeId.Instance.GetNodeIdFromIdentifier(myUserId),
                            () =>
                            {
                                GetAllAssociateEntries_Here(
                                    myUserId,
                                    out associatesInternal, 
                                    out receivedRequestsInternal, 
                                    out sentRequestsInternal
                                );
                            },
                            () => new GetAllAssociateEntriesRequest(myUserId),
                            (response) => {
                                associatesInternal = response.MyAssociates;
                                receivedRequestsInternal = response.ReceivedRequests;
                                sentRequestsInternal = response.SentRequests;
                            },
                            _CancellationTokenSourceDisposed.Token
            );
            associates = associatesInternal;
            receivedRequests = receivedRequestsInternal;
            sentRequests = sentRequestsInternal;
        }
        public UserProfileSummary[] GetPublicUserProfileSummarys(long[] userIds)
        {
            List<NodeIdAndAssociatedIds> nodeIdAndAssociatedIdss = _NodeidRangsForUserManager
                .GetNodeIdsForIdsInRange(userIds);
            if (nodeIdAndAssociatedIdss == null) return null;
            ParallelOperationResult<NodeIdAndAssociatedIds, UserProfileSummary[]>[]
                results = ParallelOperationHelper.RunInParallel(
                    nodeIdAndAssociatedIdss, (nodeIdAndAssociatedIds) => {
                        UserProfileSummary[] userProfileSummarys = null;
                        OperationRedirectHelper.OperationRedirectedToNode<
                            GetPublicUserProfileSummarysRequest, GetPublicUserProfileSummarysResponse>(
                            nodeIdAndAssociatedIds.NodeId,
                            () =>
                            {
                                userProfileSummarys = GetPublicUserProfileSummarys_Here(nodeIdAndAssociatedIds.Ids);
                            },
                            () => new GetPublicUserProfileSummarysRequest(nodeIdAndAssociatedIds.Ids),
                            (response) => {
                                userProfileSummarys = response.UserProfileSummarys;
                            },
                            _CancellationTokenSourceDisposed.Token
                            );
                        return userProfileSummarys;
                    },
                    Configurations.Threading.MAX_N_THREADS_GET_USER_PROFILE_SUMMARYS_THREAD_FOR_EACH_NODE
            );
            /*
            List<UserProfileSummary> summarys = new List<UserProfileSummary>();
            foreach (ParallelOperationResult<NodeIdAndAssociatedIds, UserProfileSummary[]> result in results)
            {
                if (result.Success)
                    summarys.AddRange(result.Return);
                else
                    Logs.Default.Error($"Failed to get {nameof(UserProfileSummary)}'s from node with {nameof(result.Argument.NodeId)} {result.Argument.NodeId}");
            }*/
            return results.Where(r => r.Success).SelectMany(r => r.Return).ToArray();
        }
        public UserProfileSummary[] GetMyAssociatesProfileSummaries(long myUserId)
        {
            Associates myAssociates = DalAssociates.Instance.GetUsersAssociates(myUserId);
            return _GetUserProfileSummarys(myAssociates?.Entries);
        }
        public UserProfileSummarysForMeToSeeOnAnotherUser GetAnotherUsersAssociatesUserProfileSummarys(long myUserId, long otherUserId)
        {
            Associates otherUsersAssociates = DalAssociates.Instance.GetUsersAssociates(otherUserId);
            otherUsersAssociates.TryGet(myUserId, out Associate myAssociateWithOtherUser);
            AssociateType myAssociateTypeWithUser = (myAssociateWithOtherUser?.AssociateType)
                ?? AssociateType.None;
            if (!otherUsersAssociates.VisibleToMe(myAssociateTypeWithUser))
                return UserProfileSummarysForMeToSeeOnAnotherUser.NotVisible();
            Associates myAssociats = DalAssociates.Instance.GetUsersAssociates(otherUserId);
            Associate[] upgradedAssociatesToConsiderSharedAssociation = 
                UpgradeAssociationTypeOnSharedAssociatesAndLimitOnRest(otherUsersAssociates, myAssociats);
            otherUsersAssociates = null; myAssociats = null;//prevent mistakes by accident. this should not be used from here.
            UserProfileSummary[] summaries = _GetUserProfileSummarys(upgradedAssociatesToConsiderSharedAssociation);
            return new UserProfileSummarysForMeToSeeOnAnotherUser(summaries);
        }
        public bool RemoveAssociate(long myUserId, long otherUserId)
        {
            bool success = false;
            try
            {
                _OperationRedirectedToNodeWithHighestUserIdForSafeDoubleLocking<RemoveAssociateRequest, SuccessTicketedResponse>(
                    myUserId, otherUserId,
                    callbackDoHere: () => {
                        _RemoveAssociateHere(myUserId, otherUserId);
                        success = true;
                    },
                    callbackCreateRequest: () => new RemoveAssociateRequest(myUserId, otherUserId),
                    didRemotely: (removeAssociateResponse) => {
                        success = removeAssociateResponse.Success;
                    }
                );
                PushToUserDevices(AssociatesOperation.Remove, myUserId, otherUserId);
            }
            catch(Exception ex) {
                Logs.Default.Error(ex);
            }
            return success;
        }
        public bool DowngradeAssociation(long myUserId, long otherUserId, AssociateType associationTypesToKeep)
        {
            bool success = false;
            try
            {
                AssociateType associateTypesRemaining = AssociateType.None;
                _OperationRedirectedToNodeWithHighestUserIdForSafeDoubleLocking<DowngradeAssociateRequest, DowngradeAssociateResponse>(
                    myUserId, otherUserId,
                    callbackDoHere: () => {
                        associateTypesRemaining = _DowngradeAssociateHere(myUserId, otherUserId, associationTypesToKeep);
                        success = true;
                    },
                    callbackCreateRequest: () => DowngradeAssociateRequest.Interserver(myUserId, otherUserId, associationTypesToKeep),
                    didRemotely: (downgradeAssociateResponse) => {
                        success = downgradeAssociateResponse.Success;
                        associateTypesRemaining = downgradeAssociateResponse.AssociateTypesRemaining;
                    }
                );
                if (success)
                {
                    if ((int)associateTypesRemaining > 0)
                    {
                        PushToUserDevices(new AssociateUpdate(AssociatesOperation.Downgrade, myUserId, otherUserId, associateTypesRemaining));
                    }
                    else
                    {
                        PushToUserDevices(new AssociateUpdate(AssociatesOperation.Remove, myUserId, otherUserId));
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            return success;
        }
        public UserProfileForMeToSeeOnAnotherUser GetUserProfile(long myUserId, long profileUserId)
        {
            UserProfile userProfile = DalUserProfiles.Instance.GetUserProfile(profileUserId);
            Associates associates = DalAssociates.Instance.GetUsersAssociates(profileUserId);
            if (associates == null || !associates.TryGet(myUserId, out Associate associateMe))
            {
                userProfile = userProfile.ToFilteredProfile(AssociateType.None);
            }
            else
            {
                userProfile = userProfile.ToFilteredProfile(associateMe.AssociateType);
            }
            return new UserProfileForMeToSeeOnAnotherUser(userProfile);
        }
        public UserProfile GetMyUserProfile(long myUserId)
        {
            UserProfile userProfile = DalUserProfiles.Instance.GetUserProfile(myUserId);
            return userProfile;
        }
        public void ModifyUserProfile(long myUserId, UserProfile userProfileChanges)
        {
            _OperationRedirectedToNodeWithUserIdForSafeDoubleLocking<ModifyUserProfileRequest, SuccessTicketedResponse>(
                myUserId,
                callbackDoHere: () => {
                    _ModifyUserProfile_Here(myUserId, userProfileChanges);
                },
                callbackCreateRequest: () => new ModifyUserProfileRequest(myUserId, userProfileChanges),
                didRemotely: (response) => {
                    if (!response.Success)
                        throw new OperationFailedException($"Failed to modify profile for {myUserId}");
                }
            );
        }
        public bool UsernameSearchSearch(string str, int maxNEntries, out long[] userIds)
        {
            long[] userIdsInternal = null;
            bool successful = false;
            OperationRedirectHelper.OperationRedirectedToNode<
                UsernameSearchSearchRequest, UsernameSearchSearchResponse>(
                _UsernameSearchIdentifierToNodeId.GetNodeIdFromIdentifier(str),
                callbackDoHere: () => {
                    successful = UsernameSearchSearch_Here(str,
                        maxNEntries, out userIdsInternal);
                },
                callbackCreateRequest: () => new UsernameSearchSearchRequest(str, maxNEntries),
                didRemotely: (response) => {
                    successful = response.Success;
                    userIdsInternal = response.UserIds;
                },
                ShutdownManager.Instance.CancellationToken
            );
            userIds = userIdsInternal;
            return successful;
        }
        public bool UsernameSearchAddUser(string username, long userId)
        {
            bool successful = false;
            OperationRedirectHelper.OperationRedirectedToNode<
                UsernameSearchAddUserRequest, UsernameSearchAddUserResponse>(
                _UsernameSearchIdentifierToNodeId.GetNodeIdFromIdentifier(username),
                callbackDoHere: () => {
                    successful = UsernameSearchAddUser_Here(username,
                        userId);
                },
                callbackCreateRequest: () => new UsernameSearchAddUserRequest(username, userId),
                didRemotely: (response) => {
                    successful = response.Successful;
                },
                ShutdownManager.Instance.CancellationToken
            );
            return successful;
        }
        public bool UsernameSearchRemoveUser(string username, long userId)
        {
            bool successful = false;
            OperationRedirectHelper.OperationRedirectedToNode<
                UsernameSearchRemoveUserRequest, UsernameSearchRemoveUserResponse>(
                _UsernameSearchIdentifierToNodeId.GetNodeIdFromIdentifier(username),
                callbackDoHere: () => {
                    successful = UsernameSearchRemoveUser_Here(userId);
                },
                callbackCreateRequest: () => new UsernameSearchRemoveUserRequest(userId),
                didRemotely: (response) => {
                    successful = response.Success;
                },
                ShutdownManager.Instance.CancellationToken
            );
            return successful;
        }

        public AssociateRequestUserProfileSummarys GetMyReceivedRequests(long myUserId)
        {
            AssociateRequest[] associateRequests = DalAssociateRequests.Instance.GetReceivedRequests(myUserId)?.Entries;
            return GetMy_Requests(associateRequests, (associateRequest) => associateRequest.AssociateType);
        }
        public AssociateRequestUserProfileSummarys GetMySentRequests(long myUserId)
        {
            AssociateRequest[] associateRequests =  DalAssociateRequests.Instance.GetSentRequests(myUserId)?.Entries;
            return GetMy_Requests(associateRequests, (associateRequest)=>AssociateType.None);
        }
        private AssociateRequestUserProfileSummarys GetMy_Requests(AssociateRequest[] associateRequests, 
            Func<AssociateRequest, AssociateType> getAssociateTypeHavePermissionsFor)
        {
            if (associateRequests == null) return new AssociateRequestUserProfileSummarys(null); ;
            Associate[] associates = associateRequests.Select(associateRequest => new Associate(associateRequest.UserId, getAssociateTypeHavePermissionsFor(associateRequest))).ToArray();
            UserProfileSummary[] userProfileSummarys = _GetUserProfileSummarys(associates);
            Dictionary<long, UserProfileSummary> mapUserIdToUserProfileSummary = userProfileSummarys
                .ToDictionary(u => u.UserId, u => u);
            AssociateRequestUserProfileSummary[] pairs = associateRequests
                .Select(associateRequest => new AssociateRequestUserProfileSummary(
                    associateRequest,
                    mapUserIdToUserProfileSummary.TryGetValue(associateRequest.UserId, out UserProfileSummary u)
                        ? u : null))
                .ToArray();
            return new AssociateRequestUserProfileSummarys(pairs);
        }
        public bool CancelSentRequest(long myUserId, long otherUserId)
        {
            bool success = false;
            try
            {
                _OperationRedirectedToNodeWithHighestUserIdForSafeDoubleLocking<CancelSentRequestRequest, SuccessTicketedResponse>(
                    myUserId, otherUserId,
                    callbackDoHere: () => {
                        _CancelSentRequestHere(myUserId, otherUserId);
                        success = true;
                    },
                    callbackCreateRequest: () => new CancelSentRequestRequest(myUserId, otherUserId),
                    didRemotely: (acceptRequestResponse) => {
                        success = acceptRequestResponse.Success;
                    }
                );
                PushToUserDevices(AssociatesOperation.Cancel, myUserId, otherUserId);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            return success;
        }
        public bool RejectRequest(long myUserId, long otherUserId)
        {
            bool success = false;
            try
            {
                _OperationRedirectedToNodeWithHighestUserIdForSafeDoubleLocking<RejectRequestRequest, SuccessTicketedResponse>(
                    myUserId, otherUserId,
                    callbackDoHere: () => {
                        _RejectRequestHere(myUserId, otherUserId);
                        success = true;
                    },
                    callbackCreateRequest: () => new RejectRequestRequest(myUserId, otherUserId),
                    didRemotely: (acceptRequestResponse) => {
                        success = acceptRequestResponse.Success;
                    }
                );
                PushToUserDevices(AssociatesOperation.Reject, myUserId, otherUserId);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            return success;
        }
        public bool AlterRequest(long myUserId,
            long otherUserId, AssociateType associationTypes)//TODO Improve efficiency by adding here method same for counter requirest
        {
            try
            {
                DowngradeAssociation(myUserId, otherUserId, associationTypes);
                Request(myUserId, otherUserId, associationTypes);
                RejectRequest(myUserId, otherUserId);//reject any pending requests cos user made it clear what they want the association to be and any overlap will habe been accepted.
                PushToUserDevices(AssociatesOperation.Counter, myUserId, otherUserId);
                return true;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
        public bool CounterRequest(long myUserId,
            long otherUserId, AssociateType counterAssociateType)
        {
            bool success = false;
            try
            {
                Request(myUserId, otherUserId, counterAssociateType);
                RejectRequest(myUserId, otherUserId);
                PushToUserDevices(AssociatesOperation.Counter, myUserId, otherUserId);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            return success;
        }
        public bool AcceptRequest(long myUserId, long otherUserId, long requestUniqueIdentifier, AssociateType? limitTo = null)
        {
            bool success = false;
            UserProfileSummary actingUserProfileSummary = null;
            UserProfileSummary otherUserProfileSummary = null;
            try
            {
                _OperationRedirectedToNodeWithHighestUserIdForSafeDoubleLocking<AcceptAssociatetRequest, AcceptRquestResponse>(
                    myUserId, otherUserId,
                    callbackDoHere: () => {
                        _AcceptRequestHere(myUserId, otherUserId, requestUniqueIdentifier, limitTo,
                            out actingUserProfileSummary, out otherUserProfileSummary);
                        success = true;
                    },
                    callbackCreateRequest: () => new AcceptAssociatetRequest(myUserId, otherUserId, requestUniqueIdentifier, limitTo),
                    didRemotely: (acceptRequestResponse) => {
                        success = acceptRequestResponse.Success;
                        actingUserProfileSummary = acceptRequestResponse.MyUserProfileSummary;
                        otherUserProfileSummary = acceptRequestResponse.OtherUserProfileSummary;
                    }
                );
                if (success)
                {
                    PushToUserDevices(new AssociateUpdate(AssociatesOperation.Accept,
                        myUserId, otherUserId, actingUserProfileSummary, otherUserProfileSummary));
                }
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            return success;
        }
        public void PushToUserDevices(AssociatesOperation operation, long actingUserId, long[] userIdsInvolved) {

            long otherUserId = userIdsInvolved.Where(u => u != actingUserId).First();
            AssociateUpdate update = new AssociateUpdate(operation, actingUserId, otherUserId);
            UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(update, userIdsInvolved);
        }
        public void PushToUserDevices(AssociatesOperation operation, long actingUserId, long otherUserId)
        {

            AssociateUpdate update = new AssociateUpdate(operation, actingUserId, otherUserId);
            UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(update, actingUserId, otherUserId);
        }
        public void PushToUserDevices(AssociateUpdate associateUpdate)
        {
            UserRoutedMessagesManager.Instance.ForwardObjectToUserDevices(associateUpdate, associateUpdate.ActingUserId, associateUpdate.OtherUserId);
        }
        public bool Request(long myUserId, long otherUserId,
            AssociateType associateTypesRequesting)
        {
            bool success = false;
            try
            {
                AssociateType allAssociateTypesRequesting = associateTypesRequesting;
                if (associateTypesRequesting <= AssociateType.None)
                    throw new Exception("Cannot request an associate with no associate type");
                if (_Request_DealWithExistingAssociation(myUserId, otherUserId,
                    ref associateTypesRequesting, ref success))
                {//TODO do we need to push here.
                    return success;
                }
                if (_Request_DealWithRequestFromOtherUser(myUserId, otherUserId,
                    ref associateTypesRequesting, ref success))
                {
                    return success;
                }
                AssociateRequestUserProfileSummary actingAssociateRequestUserProfileSummary = null;
                AssociateRequestUserProfileSummary otherUserAssociateRequestUserProfileSummary = null;
                _OperationRedirectedToNodeWithHighestUserIdForSafeDoubleLocking<RequestAssociateRequest, RquestAssociateResponse>(
                    myUserId, otherUserId,
                    callbackDoHere: () =>
                    {
                        _RequestAssociateHere(myUserId, otherUserId, associateTypesRequesting,
                            out actingAssociateRequestUserProfileSummary,
                        out otherUserAssociateRequestUserProfileSummary);
                        success = true;
                    },
                    callbackCreateRequest: () => new RequestAssociateRequest(myUserId, otherUserId, associateTypesRequesting),
                    didRemotely: (requestAssociateResponse) =>
                    {
                        success = requestAssociateResponse.Success;
                        actingAssociateRequestUserProfileSummary = requestAssociateResponse.ActingAssociateRequestUserProfileSummary;
                        otherUserAssociateRequestUserProfileSummary = requestAssociateResponse.OtherUserAssociateRequestUserProfileSummary;
                    }
                );
                //Check associate type requesting here.
                if (success)
                {
                    PushToUserDevices(new AssociateUpdate(AssociatesOperation.Request,
                        myUserId, otherUserId, associateTypesRequesting, actingAssociateRequestUserProfileSummary,
                        otherUserAssociateRequestUserProfileSummary));
                }
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            return success;
        }
        private bool _Request_DealWithExistingAssociation(long myUserId, long otherUserId,
            ref AssociateType associateTypesRequesting, ref bool success) {
            Associate associateBetweenUs = _GetAssociate(myUserId, otherUserId);
            if(associateBetweenUs==null)
                return false;
            AssociateType associateTypesRequestingWithoutExistingAssociateTypes = 
                associateTypesRequesting & (~associateBetweenUs.AssociateType);
            if (associateTypesRequestingWithoutExistingAssociateTypes <= AssociateType.None)
            {
                success = true;
                return true;
            }
            associateTypesRequesting = associateTypesRequestingWithoutExistingAssociateTypes;
            return false;
        }
        private bool _Request_DealWithRequestFromOtherUser(long myUserId, long otherUserId,
            ref AssociateType associateTypesRequesting, ref bool success) {
            AssociateRequest existingAssociateRequestToMeReceived = _GetAssociateReceivedRequest(myUserId, otherUserId);
            if (existingAssociateRequestToMeReceived == null) 
                return false;
            AssociateType overlappingAssociateTypes = existingAssociateRequestToMeReceived.AssociateType & associateTypesRequesting;
            if (overlappingAssociateTypes > AssociateType.None)
            {
                AcceptRequest(myUserId, otherUserId, 0, limitTo: overlappingAssociateTypes);
            }
            AssociateType nonOverlappingAssociateTypesIRequested = associateTypesRequesting & (~existingAssociateRequestToMeReceived.AssociateType);
            if (nonOverlappingAssociateTypesIRequested <= AssociateType.None)
            {
                success = true;
                return true;
            }
            associateTypesRequesting = nonOverlappingAssociateTypesIRequested;
            return false;
        }
        public bool Invite(long myUserId, string email,
            string phoneNumber,
            AssociateType associateType)
        {
            bool success = false;
            AuthenticationInfo authenticationInfo = null;
            if (!string.IsNullOrEmpty(email))
            {
                authenticationInfo = DalAuthentication.Instance.GetAuthenticationInfoByEmail(email);
            }
            else {
                if (!string.IsNullOrEmpty(phoneNumber)) {
                    authenticationInfo = DalAuthentication.Instance.GetAuthenticationInfoByPhone(phoneNumber);
                }
            }
            if (authenticationInfo == null)
                throw new InviteException(InviteFailedReason.CannotFindUser);
            long otherUserId = authenticationInfo.UserId;
            if (myUserId == otherUserId)
                throw new InviteException(InviteFailedReason.CannotInviteYourself);
            Request(myUserId, otherUserId,
                associateType);
            return success;
        }
        /*
        private void _PushUpdateNotificationToClientsWithUserIdsBothWaysRoundOnNewThread(AssociateUpdateType associateUpdateType,
            long userIdA, long userIdB)
        {

            new Thread(() =>
            {
                try
                {
                    _PushUpdateNotificationToClients(associateUpdateType, userIdA, userIdB);
                    _PushUpdateNotificationToClients(associateUpdateType, userIdB, userIdA);
                }
                catch (Exception ex)
                {
                    Logs.AssociateUpdate?.Error(ex);
                }
            }).Start();
        }
        private void _PushUpdateNotificationToClientsOnNewThread(AssociateUpdateType associateUpdateType, long toUserId, long userIdUpdated)
        {

            new Thread(() =>
            {
                try
                {
                    _PushUpdateNotificationToClients(associateUpdateType, toUserId, userIdUpdated);
                }
                catch (Exception ex)
                {
                    Logs.AssociateUpdate?.Error(ex);
                }
            }).Start();
        }
        private void _PushUpdateNotificationToClients(AssociatesOperation associateUpdateType, long toUserId, long userIdUpdated)
        {
            AssociateUpdate update = new AssociateUpdate(toUserId, userIdUpdated, associateUpdateType);
            string content = Json.Serialize(update);
            NodeIdSessionIdPair[] nodeIdSessionIdPairs = CoreUserRoutingTable
                .Instance.GetNodeIdSessionIdPairs(toUserId);
            if (nodeIdSessionIdPairs == null) return;
            foreach (NodeIdSessionIdPair nodeIdSessionIdPair in nodeIdSessionIdPairs)
            {
                INodeEndpoint nodeEndpoint = InterserverPort.Instance.InterserverEndpoints.GetEndpoint(nodeIdSessionIdPair.NodeId);
                if (nodeEndpoint == null)
                {
                    Logs.AssociateUpdate.Error(new OperationFailedException($"Could not get endpointy for  node {nodeIdSessionIdPair.NodeId}"));
                    continue;
                }
                try
                {
                    nodeEndpoint.SendJSONString(content);
                }
                catch (Exception ex)
                {
                    Logs.AssociateUpdate.Error(ex);
                }
            }
        }*/
        #endregion Public
        #region Private
        private void _OperationRedirectedToNodeWithUserIdForSafeDoubleLocking<TRemoteRequest, TRemoteResponse>(
            long myUserId, Action callbackDoHere,
            Func<TRemoteRequest> callbackCreateRequest,
            Action<TRemoteResponse> didRemotely)
            where TRemoteRequest : TicketedMessageBase where TRemoteResponse : TicketedMessageBase
        {
            INode node = _NodeidRangsForUserManager.GetNodeForIdInRange(myUserId);
            if (node == null) throw new OperationFailedException($"Could not find a node for the userId {myUserId}");
            if (node.Id == _MyNodeId)
            {
                callbackDoHere();
                return;
            }
            INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(node.Id);
            if (nodeEndpoint == null)
                throw new OperationFailedException($"Failed to get {nameof(INodeEndpoint)} for node with id {node.Id}");
            TRemoteResponse removeAssociateResponse = InterserverTicketedSender.Send<TRemoteRequest, TRemoteResponse>(
                callbackCreateRequest(),
                Configurations.Timeouts.TIMEOUT_REMOTE_DOUBLE_LOCK_OPERATION, _CancellationTokenSourceDisposed.Token, nodeEndpoint.SendJSONString);
            didRemotely(removeAssociateResponse);
        }
        private void _OperationRedirectedToNodeWithHighestUserIdForSafeDoubleLocking<TRemoteRequest, TRemoteResponse>(
            long myUserId, long otherUserId, Action callbackDoHere,
            Func<TRemoteRequest> callbackCreateRequest,
            Action<TRemoteResponse> didRemotely)
            where TRemoteRequest : TicketedMessageBase where TRemoteResponse : TicketedMessageBase
        {
            if (myUserId == otherUserId)
                throw new ArgumentException($"Something went wrong. Both userId's were the same {myUserId},{otherUserId}");
            long highestUserId = myUserId < otherUserId ? otherUserId : myUserId;
            INode nodeHighestUserId = _NodeidRangsForUserManager.GetNodeForIdInRange(highestUserId);
            if (nodeHighestUserId == null) throw new OperationFailedException($"Could not find a node for the userId {highestUserId}");
            if (nodeHighestUserId.Id == _MyNodeId)
            {
                callbackDoHere();
                return;
            }
            INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(nodeHighestUserId.Id);
            if (nodeEndpoint == null)
                throw new OperationFailedException($"Failed to get {nameof(INodeEndpoint)} for node with id {nodeHighestUserId.Id}");
            TRemoteResponse removeAssociateResponse = InterserverTicketedSender.Send<TRemoteRequest, TRemoteResponse>(
                callbackCreateRequest(),
                Configurations.Timeouts.TIMEOUT_REMOTE_DOUBLE_LOCK_OPERATION, _CancellationTokenSourceDisposed.Token, nodeEndpoint.SendJSONString);
            didRemotely(removeAssociateResponse);
        }
        private Associate[] UpgradeAssociationTypeOnSharedAssociatesAndLimitOnRest(
            Associates otherUsersAssociates, Associates myAssociates)
        {
            Associate[] otherUsersAssociatesEntries = otherUsersAssociates.Entries;
            Associate[] upgradedAssociates = new Associate[otherUsersAssociatesEntries.Length];
            for (int i = 0; i < otherUsersAssociatesEntries.Length; i++)
            {
                Associate otherUsersAssociate = otherUsersAssociatesEntries[i];
                if (myAssociates.TryGet(otherUsersAssociate.UserId, out Associate asMyAssociate))
                {
                    upgradedAssociates[i] = new Associate(otherUsersAssociate.UserId, asMyAssociate.AssociateType);
                    continue;
                }
                upgradedAssociates[i] = new Associate(otherUsersAssociate.UserId, AssociateType.None);
            }
            return upgradedAssociates;
        }
        private UserProfileSummary[] _GetUserProfileSummarys(Associate[] associates)
        {

            NodeIdAssociatesPair[] nodeIdAssociatesPairs = _GetNodeIdAssociatesPairs(associates);
            if (nodeIdAssociatesPairs == null) return null;
            ParallelOperationResult<NodeIdAssociatesPair, UserProfileSummary[]>[]
                results = ParallelOperationHelper.RunInParallel(
                    nodeIdAssociatesPairs, (nodeIdAssociatesPair) => {
                        if (nodeIdAssociatesPair.NodeId == _MyNodeId)
                        {
                            return _GetUserProfileSummarysWhichAreLocal(nodeIdAssociatesPair.Associates);
                        }
                        return _GetUserProfileSummarysFromRemoteMachine(nodeIdAssociatesPair.NodeId,
                            nodeIdAssociatesPair.Associates);
                    }, Configurations.Threading.MAX_N_THREADS_GET_USER_PROFILE_SUMMARYS_THREAD_FOR_EACH_NODE);
            List<UserProfileSummary> summarys = new List<UserProfileSummary>();
            foreach (ParallelOperationResult<NodeIdAssociatesPair, UserProfileSummary[]> result in results)
            {
                if (result.Success)
                    summarys.AddRange(result.Return);
                else
                    Logs.Default.Error($"Failed to get {nameof(UserProfileSummary)}'s from node with {nameof(result.Argument.NodeId)} {result.Argument.NodeId}");
            }
            return summarys.ToArray();
        }
        private  NodeIdAssociatesPair[] _GetNodeIdAssociatesPairs(Associate[] associates) {
            Nodes.Nodes nodes = Nodes.Nodes.Instance;
            Dictionary<int, List<Associate>> mapNodeIdToAssociates = new Dictionary<int, List<Associate>>();
            List<long> couldNotIdentifyNodeFor = null;
            if (associates == null) return null;
            foreach (Associate associate in associates) {
                INode node = _NodeidRangsForUserManager.GetNodeForIdInRange(associate.UserId);
                if (node == null) {
                    if (couldNotIdentifyNodeFor == null) couldNotIdentifyNodeFor = new List<long> { associate.UserId};
                    else couldNotIdentifyNodeFor.Add(associate.UserId);
                    continue;
                }
                if (!mapNodeIdToAssociates.ContainsKey(node.Id)) {
                    mapNodeIdToAssociates[node.Id] = new List<Associate> { associate };
                    continue;
                }
                mapNodeIdToAssociates[node.Id].Add(associate);
            }
            if (couldNotIdentifyNodeFor != null)
            {
                Logs.Default.Error($"Could not identify a node for {nameof(Associate)} with {nameof(Associate.UserId)}'s {(string.Join(",", couldNotIdentifyNodeFor))}");
            }
            return mapNodeIdToAssociates.Select(p => new NodeIdAssociatesPair(p.Key, p.Value.ToArray())).ToArray();
        }
        private UserProfileSummary[] _GetUserProfileSummarysFromRemoteMachine(int nodeId, Associate[] associates) {
            INodeEndpoint nodeEndpoint = InterserverPort.Instance.InterserverEndpoints.GetEndpoint(nodeId);
            if (nodeEndpoint == null) {
                throw new OperationFailedException($"Failed to get a range of {nameof(UserProfileSummary)}'s since {nameof(nodeEndpoint)} was null for {nameof(nodeId)} {nodeId}");
            }
            GetUserProfileSummarysResponse result = InterserverTicketedSender.Send<GetUserProfileSummarysRequest, GetUserProfileSummarysResponse>(
                    new GetUserProfileSummarysRequest(associates),
                    Configurations.Timeouts.GET_USER_PROFILE_SUMMARYS_INTERSERVER,
                    _CancellationTokenSourceDisposed.Token, 
                    nodeEndpoint.SendJSONString
                );
            if (result.UserIdsCouldNotGet != null&& result.UserIdsCouldNotGet.Length>0)
            {
                Logs.Default.Error($"Could not get {nameof(UserProfile)} for the following {nameof(Associate.UserId)}'s: {(string.Join(",", result.UserIdsCouldNotGet))}");
            }
            return result.UserProfileSummarys;
        }
        private UserProfileSummary[] _GetUserProfileSummarysWhichAreLocal(Associate[] associates) {
            ParallelOperationResult<Associate, UserProfileSummary>[]
                results = ParallelOperationHelper.RunInParallel(
                    associates, (associate) => {
                        UserProfile userProfile = DalUserProfiles.Instance.GetUserProfile(associate.UserId);
                       UserProfileSummary userProfileSummary = userProfile?.ToSummary(associate.AssociateType);
                        return userProfileSummary;
                    }, Configurations.Threading.MAX_N_THREADS_GET_USER_PROFILE_SUMMARYS_WHICH_ARE_LOCAL);
            List<long> hadNoUserProfileFor = null;
            List<UserProfileSummary>  successes = new List<UserProfileSummary>();

            foreach (ParallelOperationResult<Associate, UserProfileSummary> result in results) {
                if (result.Success&& (result.Return!=null)) {
                    successes.Add(result.Return);
                    continue;
                }
                if (hadNoUserProfileFor == null) hadNoUserProfileFor = new List<long> { result.Argument.UserId };
                else hadNoUserProfileFor.Add(result.Argument.UserId);
            }
            if (hadNoUserProfileFor != null)
            {
                Logs.Default.Error($"Had no {nameof(UserProfile)} for the following {nameof(Associate.UserId)}'s: {(string.Join(",", hadNoUserProfileFor))}");
            }
            return successes.ToArray();
        }
        private Associate _GetAssociate(long myUserId, long otherUserId)
        {

            Associate associate = null;
            _OperationRedirectedToNodeWithUserIdForSafeDoubleLocking<GetAssociateRequest, GetAssociateResponse>(
                myUserId,
                callbackDoHere: () => {
                    associate = _GetAssociate_Here(myUserId, otherUserId);
                },
                callbackCreateRequest: () => new GetAssociateRequest(myUserId, otherUserId),
                didRemotely: (getAssociateResponse) => {
                    associate = getAssociateResponse.Associate;
                }
            );
            return associate;
        }
        private AssociateRequest _GetAssociateReceivedRequest(long onAssociateUserId, long fromAssociateUserId)
        {

            AssociateRequest associateRequestReceived = null;
            _OperationRedirectedToNodeWithUserIdForSafeDoubleLocking<GetAssociateReceivedRequestRequest, GetAssociateRequestResponse>(
                onAssociateUserId,
                callbackDoHere: () => {
                    associateRequestReceived = _GetAssociateRequestReceived_Here(onAssociateUserId, fromAssociateUserId);
                },
                callbackCreateRequest: () => new GetAssociateReceivedRequestRequest(onAssociateUserId, fromAssociateUserId),
                didRemotely: (getAssociateRequestResponse) => {
                    associateRequestReceived = getAssociateRequestResponse.AssociateRequest;
                }
            );
            return associateRequestReceived;
        }
        private void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
        }

        public string GetInfoForEmailing(long userId)
        {
            return GetPublicUserProfileSummarys(new long[] { userId })?.FirstOrDefault()?.Username;
        }
        #endregion Private
        #endregion Methods
    }
}
using JSON;
using Core.Handlers;
using Logging;
using Users.Messages.Interserver;
using Users.Messages.Client;
using Core.Messages.Responses;
using InterserverComs;
using MessageTypes.Internal;
using Core.Interfaces;
using Core;
using UserRouting;
using System.Net.Sockets;
using UsersEnums;

namespace Users
{
    public partial class UsersMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    { InterserverMessageTypes.UsersGetPublicUserProfileSummarys, HandleGetPublicUserProfileSummarys},
                    { InterserverMessageTypes.UsersGetUserProfileSummarys,HandleGetUserProfileSummarys },
                    { InterserverMessageTypes.UsersRemoveAssociate, HandleRemoveAssociate},
                    { InterserverMessageTypes.UsersDowngradeAssociate, HandleDowngradeAssociate},
                    { InterserverMessageTypes.UsersCancelSentRequest, HandleCancelSentRequest},
                    //{ InterserverMessageTypes.UsersCounterRequest, HandleCounterRequest},Taken care of as two higher level operations
                    { InterserverMessageTypes.UsersRejectRequest, HandleRejectRequest},
                    { InterserverMessageTypes.UsersAcceptRequest, HandleAcceptRequest},
                    { InterserverMessageTypes.UsersRequestAssociate, HandleRequestAssociate},
                    //{ InterserverMessageTypes.UsersAssociateUpdate, HandleAssociateUpdate},
                    { InterserverMessageTypes.UsersGetAssociate, HandleGetAssociate },
                    { InterserverMessageTypes.UserGetAssociateReceivedRequest, HandleGetAssociateReceivedRequest},
                    { InterserverMessageTypes.UserModifyUserProfile, HandleModifyUserProfile},
                    { InterserverMessageTypes.UsernameSearchSearch , HandleUsernameSearchSearch},
                    { InterserverMessageTypes.UsernameSearchAddUser , HandleUsernameSearchAddUser},
                    { InterserverMessageTypes.UsernameSearchRemoveUser, HandleUsernameSearchRemoveUser},
                    { InterserverMessageTypes.UsersGetAllAssociateEntries, HandleGetAllAssociateEntries}
                }
            );
        }
        private void HandleGetAllAssociateEntries(InterserverMessageEventArgs e)
        {
            GetAllAssociateEntriesRequest request = e.Deserialize<GetAllAssociateEntriesRequest>();
            GetAllAssociateEntries_Here(
                request.MyUserId, 
                out UserProfileSummary[] associates,
                out AssociateRequestUserProfileSummarys receivedRequests,
                out AssociateRequestUserProfileSummarys sentRequests);
            GetAllAssociateEntriesResponse response = new GetAllAssociateEntriesResponse(
                associates, receivedRequests, sentRequests, request.Ticket);
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleModifyUserProfile(InterserverMessageEventArgs e)
        {
            ModifyUserProfileRequest request = e.Deserialize<ModifyUserProfileRequest>();
            DoSendingSuccessTicketedResponse(() =>
                _ModifyUserProfile_Here(request.MyUserId, request.UserProfileChanges), e.EndpointFrom, request.Ticket);
        }
        private void HandleGetAssociate(InterserverMessageEventArgs e)
        {
            GetAssociateRequest request = e.Deserialize<GetAssociateRequest>();
            Associate associate  = _GetAssociate_Here(request.MyUserId, request.OtherUserId);
            GetAssociateResponse response = new GetAssociateResponse(associate, request.Ticket);
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleGetAssociateReceivedRequest(InterserverMessageEventArgs e)
        {
            GetAssociateReceivedRequestRequest request = e.Deserialize<GetAssociateReceivedRequestRequest>();
            AssociateRequest associateRequest = _GetAssociateRequestReceived_Here(request.OnAssociateUserId, request.FromAssociateUserId);
            GetAssociateRequestResponse response = new GetAssociateRequestResponse(associateRequest, request.Ticket);
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleDowngradeAssociate(InterserverMessageEventArgs e)
        {
            DowngradeAssociateRequest request = e.Deserialize<DowngradeAssociateRequest>();
            DowngradeAssociateResponse response;
            try
            {
                AssociateType associateTypesRemaining = _DowngradeAssociateHere(request.MyUserId, request.OtherUserId, request.AssociationTypesToKeep);
                response = new DowngradeAssociateResponse(true, associateTypesRemaining, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new DowngradeAssociateResponse(false, AssociateType.None, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleCancelSentRequest(InterserverMessageEventArgs e)
        {
            CancelSentRequestRequest request = e.Deserialize<CancelSentRequestRequest>();
            DoSendingSuccessTicketedResponse(
                () => _CancelSentRequestHere(request.MyUserId, request.OtherUserId),
                e.EndpointFrom, request.Ticket);
        }
        private void HandleRejectRequest(InterserverMessageEventArgs e)
        {
            RejectRequestRequest request = e.Deserialize<RejectRequestRequest>();
            DoSendingSuccessTicketedResponse(
                () => _RejectRequestHere(request.MyUserId, request.OtherUserId),
                e.EndpointFrom, request.Ticket);
        }
        private void HandleAcceptRequest(InterserverMessageEventArgs e)
        {
            AcceptAssociatetRequest request = e.Deserialize<AcceptAssociatetRequest>();
            AcceptRquestResponse response;
            try
            {
                _AcceptRequestHere(request.MyUserId, request.OtherUserId, request.RequestUniqueIdentifier, request.LimitTo,
                    out UserProfileSummary myUserProfileSummary, out UserProfileSummary otherUserProfileSummary);
                response = new AcceptRquestResponse(true, myUserProfileSummary, otherUserProfileSummary, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new AcceptRquestResponse(false, null, null, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleRequestAssociate(InterserverMessageEventArgs e)
        {
            RequestAssociateRequest request = e.Deserialize<RequestAssociateRequest>();
            RquestAssociateResponse response;
            try
            {
                _RequestAssociateHere(request.MyUserId, request.OtherUserId, request.AssociateType,
                    out AssociateRequestUserProfileSummary actingAssociateRequestUserProfileSummary,
                    out AssociateRequestUserProfileSummary otherUserAssociateRequestUserProfileSummary);
                response = new RquestAssociateResponse(true, actingAssociateRequestUserProfileSummary,
                    otherUserAssociateRequestUserProfileSummary, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new RquestAssociateResponse(false, null, null, request.Ticket);
            }
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleRemoveAssociate(InterserverMessageEventArgs e)
        {
            RemoveAssociateRequest request = e.Deserialize<RemoveAssociateRequest>();
            DoSendingSuccessTicketedResponse(
                () => _RemoveAssociateHere(request.MyUserId, request.OtherUserId),
                e.EndpointFrom, request.Ticket);
        }
        private void HandleGetPublicUserProfileSummarys(InterserverMessageEventArgs e)
        {
            GetPublicUserProfileSummarysRequest request = Json
                .Deserialize<GetPublicUserProfileSummarysRequest>(e.JsonString);
            UserProfileSummary[] userProfileSummarys = GetPublicUserProfileSummarys_Here(request.UserIds);
            GetPublicUserProfileSummarysResponse response = new GetPublicUserProfileSummarysResponse(userProfileSummarys, request.Ticket);
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleGetUserProfileSummarys(InterserverMessageEventArgs e)
        {
            GetUserProfileSummarysRequest request = Json
                .Deserialize<GetUserProfileSummarysRequest>(e.JsonString);
            UserProfileSummary[] userProfileSummarys = _GetUserProfileSummarysWhichAreLocal(request.Associates);
            GetUserProfileSummarysResponse response = new GetUserProfileSummarysResponse(userProfileSummarys, request.Ticket);
            e.EndpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void DoSendingSuccessTicketedResponse(Action callbackDoOperation, INodeEndpoint endpointFrom, long ticket)
        { 
            SuccessTicketedResponse response;
            try
            {
                callbackDoOperation();
                response = new SuccessTicketedResponse(true, ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new SuccessTicketedResponse(false, ticket);
            }
            endpointFrom.SendJSONString(Json.Serialize(response));
        }
        private void HandleUsernameSearchSearch(InterserverMessageEventArgs e)
        {
            UsernameSearchSearchRequest request = e.Deserialize<UsernameSearchSearchRequest>();
            try
            {
                bool success = UsernameSearchSearch_Here(request.Str, request.MaxNEntries, out long[] userIds);
                e.EndpointFrom.SendJSONString(Json.Serialize(new UsernameSearchSearchResponse(userIds, success, request.Ticket)));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                e.EndpointFrom.SendJSONString(Json.Serialize(new UsernameSearchSearchResponse(null, false, request.Ticket)));
            }
        }
        private void HandleUsernameSearchAddUser(InterserverMessageEventArgs e)
        {
            UsernameSearchAddUserRequest request = e.Deserialize<UsernameSearchAddUserRequest>();
            try
            {
                bool success = UsernameSearchAddUser_Here(request.Username, request.UserId);
                e.EndpointFrom.SendJSONString(Json.Serialize(new UsernameSearchAddUserResponse(success, request.Ticket)));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                e.EndpointFrom.SendJSONString(Json.Serialize(new UsernameSearchAddUserResponse(false, request.Ticket)));
            }
        }
        private void HandleUsernameSearchRemoveUser(InterserverMessageEventArgs e)
        {
            UsernameSearchRemoveUserRequest request = e.Deserialize<UsernameSearchRemoveUserRequest>();
            try
            {
                bool success = UsernameSearchRemoveUser_Here(request.UserId);
                e.EndpointFrom.SendJSONString(Json.Serialize(new UsernameSearchRemoveUserResponse(success, request.Ticket)));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                e.EndpointFrom.SendJSONString(Json.Serialize(new UsernameSearchRemoveUserResponse(false, request.Ticket)));
            }
        }
    }
}
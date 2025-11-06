using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using Core.Messages.Responses;
using JSON;
using Users.Messages.Client;
using Logging;
using Users.Messages.Interserver;
using Core.Interfaces;
using Users.Users;
using Core;
using UserLocation;
using LocationCore;

namespace Users
{
    public class AssociatesClientEndpoint
    {
        private IClientEndpoint _Endpoint;
        private long _MyUserId { get { return _Endpoint.UserId; } }
        private Action _RemoveClientMessageTypeMappings;
        public AssociatesClientEndpoint(
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpoint snippetsClientEndpoint)
        {
            _Endpoint = snippetsClientEndpoint;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> { 
                { MessageTypes.UsersGetAllAssociateEntries, GetAllAssociateEntries},
                { MessageTypes.UsersGetPublicUserProfileSummarys, GetPublicUserProfileSummarys},
                { MessageTypes.UsersGetMyAssociatesProfileSummaries, GetMyAssociatesProfileSummaries},
                { MessageTypes.UsersGetAnotherUsersAssociatesUserProfileSummarys,GetAnotherUsersAssociatesUserProfileSummarys},
                { MessageTypes.UsersRemoveAssociate,RemoveAssociate},
                { MessageTypes.UsersDowngradeAssociation,DowngradeAssociation},
                { MessageTypes.UsersGetUserProfile,GetUserProfile},
                { MessageTypes.UsersGetMyUserProfile,GetMyUserProfile},
                { MessageTypes.UsersModifyUserProfile,ModifyUserProfile},
                { MessageTypes.UsersGetMyReceivedRequests, GetMyReceivedRequests},
                { MessageTypes.UsersGetMySentRequests, GetMySentRequests},
                { MessageTypes.UsersCancelSentRequest,CancelSentRequest},
                { MessageTypes.UsersRejectRequest, RejectRequest},
                { MessageTypes.UsersCounterRequest, CounterRequest},
                { MessageTypes.UsersAlterAssociate, AlterRequest},
                { MessageTypes.UsersAcceptRequest, AcceptRequest},
                { MessageTypes.UsersRequestAssociate, RequestAssociate},
                { MessageTypes.UsersInviteAssociateByUserId, InviteAssociate},
                { MessageTypes.UsernameSearchSearch, HandleUsernameSearchSearch},
                { UserLocation.MessageTypes.UserQuadTreeGet, HandleUserQuadTreeGet},
                { UserLocation.MessageTypes.UserQuadTreeGetNEntries, HandleUserQuadTreeGetNEntries},
                { UserLocation.MessageTypes.UserQuadTreeSet, HandleUserQuadTreeSet}
            });
        }
        public void GetAllAssociateEntries(TypeTicketedAndWholePayload message) {

            _DoInTryCatchAndSendResponse(message, () =>
            {
                UsersMesh.Instance.GetAllAssociateEntries(_MyUserId, out UserProfileSummary[] associates, out AssociateRequestUserProfileSummarys receivedRequests, 
                    out AssociateRequestUserProfileSummarys sentRequests);
                return new GetAllAssociateEntriesResponse(associates, receivedRequests, sentRequests, message.Ticket);
            });
        }
        private void GetMyAssociatesProfileSummaries(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                UserProfileSummary[] userProfileSummarys =
                    UsersMesh.Instance.GetMyAssociatesProfileSummaries(_MyUserId);
                return new GetMyAssociatesProfileSummariesResponse(userProfileSummarys);
            });
        }
        private void GetPublicUserProfileSummarys(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                GetPublicUserProfileSummarysRequest request = Json.Deserialize<GetPublicUserProfileSummarysRequest>(message.JsonString);
                UserProfileSummary[] userProfileSummarys =
                    UsersMesh.Instance.GetPublicUserProfileSummarys(request.UserIds);
                return new GetPublicUserProfileSummarysResponse(userProfileSummarys, request.Ticket);
            });
        }
        private void GetAnotherUsersAssociatesUserProfileSummarys(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                GetAnotherUsersAssociatesUserProfileSummarysRequest request = Json
                .Deserialize<GetAnotherUsersAssociatesUserProfileSummarysRequest>(message.JsonString);
                UserProfileSummarysForMeToSeeOnAnotherUser userProfileSummaryFOrMeToSeeOnAnotherUser =
                    UsersMesh.Instance.GetAnotherUsersAssociatesUserProfileSummarys(
                        _MyUserId, request.OtherUserId);
                return userProfileSummaryFOrMeToSeeOnAnotherUser;
            });
        }
        private void RemoveAssociate(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                RemoveAsssociateRequest request = Json
                .Deserialize<RemoveAsssociateRequest>(message.JsonString);
                bool removed =
                    UsersMesh.Instance.RemoveAssociate(
                        _MyUserId, request.OtherUserId);
                return removed;
            });
        }
        private void DowngradeAssociation(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                DowngradeAssociateRequest request = Json
                .Deserialize<DowngradeAssociateRequest>(message.JsonString);
                bool removed =
                    UsersMesh.Instance.DowngradeAssociation(
                        _MyUserId, request.OtherUserId, request.AssociationTypesToKeep);
                return removed;
            });
        }
        private void GetUserProfile(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                GetUserProfileRequest request = Json
                .Deserialize<GetUserProfileRequest>(message.JsonString);
                UserProfileForMeToSeeOnAnotherUser userProfile = UsersMesh.Instance.GetUserProfile(
                        _MyUserId, request.ProfileUserId);
                return userProfile;
            });
        }
        private void GetMyUserProfile(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                return UsersMesh.Instance.GetMyUserProfile(_MyUserId);
            });
        }
        private void ModifyUserProfile(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                ModifyUserProfileRequest request = Json
                .Deserialize<ModifyUserProfileRequest>(message.JsonString);
                UsersMesh.Instance.ModifyUserProfile(
                    _MyUserId, request.UserProfileChanges);
                return true;
            });
        }
        private void GetMyReceivedRequests(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                return UsersMesh.Instance.GetMyReceivedRequests(_MyUserId);
            });
        }
        private void GetMySentRequests(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                return UsersMesh.Instance.GetMySentRequests(_MyUserId);
            });
        }
        private void CancelSentRequest(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                CancelSentRequestRequest request = Json
                .Deserialize<CancelSentRequestRequest>(message.JsonString);
                bool success = UsersMesh.Instance.CancelSentRequest(_MyUserId/*DO NOT CHANGE THIS TO USE REQUEST UserId. Dangerous*/,
                    request.OtherUserId);
                return success;
            });
        }
        private void RejectRequest(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                RejectRequestRequest request = Json
                .Deserialize<RejectRequestRequest>(message.JsonString);
                bool success = UsersMesh.Instance.RejectRequest(_MyUserId/*DO NOT CHANGE THIS TO USE REQUEST UserId. Dangerous*/,
                    request.OtherUserId);
                return success;
            });
        }
        private void AlterRequest(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                AlterAssociationRequest request = Json
                .Deserialize<AlterAssociationRequest>(message.JsonString);
                bool success = UsersMesh.Instance.AlterRequest(_MyUserId/*DO NOT CHANGE THIS TO USE REQUEST UserId. Dangerous*/,
                    request.OtherUserId, request.AssociateType);
                return success;
            });
        }
        private void CounterRequest(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                CounterRequestRequest request = Json
                .Deserialize<CounterRequestRequest>(message.JsonString);
                bool success = UsersMesh.Instance.CounterRequest(_MyUserId/*DO NOT CHANGE THIS TO USE REQUEST UserId. Dangerous*/,
                    request.OtherUserId, request.CounterAssociateType);
                return success;
            });
        }
        private void AcceptRequest(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                AcceptAssociatetRequest request = Json
                .Deserialize<AcceptAssociatetRequest>(message.JsonString);
                if (request.RequestUniqueIdentifier <= 0) throw new ArgumentException($"{request.RequestUniqueIdentifier} cannot be less than 1");
                bool success = UsersMesh.Instance.AcceptRequest(_MyUserId/*DO NOT CHANGE THIS TO USE REQUEST UserId. Dangerous*/,
                    request.OtherUserId, request.RequestUniqueIdentifier);
                return success;
            });
        }
        private void RequestAssociate(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                RequestAssociateRequest request = Json
                .Deserialize<RequestAssociateRequest>(message.JsonString);
                bool success = UsersMesh.Instance.Request(_MyUserId/*DO NOT CHANGE THIS TO USE REQUEST UserId. Dangerous*/,
                    request.OtherUserId, request.AssociateType);
                return success;
            });
        }
        private void InviteAssociate(TypeTicketedAndWholePayload message)
        {
            _DoInTryCatchAndSendResponse(message, () =>
            {
                try
                {
                    InviteAssociateRequest request = Json
                    .Deserialize<InviteAssociateRequest>(message.JsonString);
                    bool success = UsersMesh.Instance.Invite(_MyUserId/*DO NOT CHANGE THIS TO USE REQUEST UserId. Dangerous*/,
                        request.Email, request.PhoneNumber, request.AssociateType);
                    return InviteAssociateResponse.Successful();
                }
                catch (InviteException iEx) {
                    return InviteAssociateResponse.Failed(iEx.FailedReason, 0);
                }
            });
        }
        private void HandleUsernameSearchSearch(TypeTicketedAndWholePayload message)
        {
            UsernameSearchSearchRequest request = Json
                .Deserialize<UsernameSearchSearchRequest>(message.JsonString);
            try
            {
                bool success = UsersMesh.Instance.UsernameSearchSearch(request.Str, Configurations.Lengths.USERNAME_SEARCH_MAX_N_ENTRIES_RESULT, out long[] userIds);
                if (!success)
                {
                    _Endpoint.SendObject(new UsernameSearchSearchResponse(null, false, request.Ticket));
                    return;
                }
                _Endpoint.SendObject(new UsernameSearchSearchResponse(userIds, true, request.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                _Endpoint.SendObject(new UsernameSearchSearchResponse(null, false, request.Ticket));
            }
        }
        private void HandleUserQuadTreeGet(TypeTicketedAndWholePayload message)
        {
            UserQuadTreeGetRequest request = Json
                .Deserialize<UserQuadTreeGetRequest>(message.JsonString);
            try
            {
                Quadrant[] quadrants =  request.LevelQuadrantPairs!=null
                    ?
                    UserQuadTree.Instance.Get(request.LevelQuadrantPairs)
                    :
                    UserQuadTree.Instance.Get(request.LatLng, (double)request.RadiusKm);
                _Endpoint.SendObject(UserQuadTreeGetResponse.Successful(quadrants, request.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                _Endpoint.SendObject(UserQuadTreeGetResponse.Failure(request.Ticket));
            }
        }
        private void HandleUserQuadTreeGetNEntries(TypeTicketedAndWholePayload message)
        {
            UserQuadTreeGetNEntriesRequest request = Json
                .Deserialize<UserQuadTreeGetNEntriesRequest>(message.JsonString);
            try
            {
                QuadrantNEntries[] quadrantNEntries = UserQuadTree.Instance.GetNEntries(request.Level, request.Quadrants, request.WithLatLng);
                _Endpoint.SendObject(UserQuadTreeGetNEntriesResponse.Successful(quadrantNEntries, request.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                _Endpoint.SendObject(UserQuadTreeGetNEntriesResponse.Failure(request.Ticket));
            }
        }
        private void HandleUserQuadTreeSet(TypeTicketedAndWholePayload message)
        {
            UserQuadTreeSetRequest request = Json
                .Deserialize<UserQuadTreeSetRequest>(message.JsonString);
            try
            {
                UserQuadTree.Instance.Set(_MyUserId, request.LatLng);
                _Endpoint.SendObject(UserQuadTreeSetResponse.Successful(request.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                _Endpoint.SendObject(UserQuadTreeSetResponse.Failure(request.Ticket));
            }
        }
        private void _DoInTryCatchAndSendResponse(TypeTicketedAndWholePayload message, Func<bool> callback)
        {

            try
            {
                bool success = callback();
                if (success)
                {
                    _Endpoint.SendObject(GenericTicketedResponse.Success(message.Ticket));
                    return;
                }
            }
            catch(Exception ex)
            {
                Logs.Default.Error(ex);
            }
            _Endpoint.SendObject(GenericTicketedResponse.Failure("An error occured", message.Ticket));
        }
        private void _DoInTryCatchAndSendResponse<TResponse>(TypeTicketedAndWholePayload message, Func<TResponse> callback) where TResponse : class
        {

            try
            {
                TResponse response = callback();
                _Endpoint.SendObject(GenericTicketedResponse.Success(response, message.Ticket));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                _Endpoint.SendObject(GenericTicketedResponse.Failure("An error occured", message.Ticket));
            }
        }
        public void Dispose() {
            _RemoveClientMessageTypeMappings();
        }
    }
}
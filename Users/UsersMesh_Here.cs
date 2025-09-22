using Logging;
using Users.DAL;
using Core.Threading;
using UsersEnums;
using UserLocation;

namespace Users
{
    public partial class UsersMesh
    {
        public void GetAllAssociateEntries_Here(long myUserId, out UserProfileSummary[] associates,
            out AssociateRequestUserProfileSummarys receivedRequests,
            out AssociateRequestUserProfileSummarys sentRequests)
        {

            associates = UsersMesh.Instance.GetMyAssociatesProfileSummaries(myUserId);
            receivedRequests = UsersMesh.Instance.GetMyReceivedRequests(myUserId);
            sentRequests = UsersMesh.Instance.GetMySentRequests(myUserId);
        }
        private UserProfileSummary[] GetPublicUserProfileSummarys_Here(long[] userIds) {
            ParallelOperationResult<long, UserProfileSummary>[] result = ParallelOperationHelper.RunInParallel(
                    userIds, (userId) => {
                        UserProfile userProfile = DalUserProfiles.Instance.GetUserProfile(userId);
                        //TODO deal with profile missing
                        UserProfileSummary userProfileSummary = userProfile?.ToPublicSummary(AssociateType.None);
                        return userProfileSummary;
                    }, GlobalConstants.Threading.MAX_N_THREADS_GET_USER_PROFILE_SUMMARYS_WHICH_ARE_LOCAL); foreach (long userId in userIds) {
            }
            return result.Where(r => r.Success).Select(r => r.Return).ToArray();
        }
        /*
        private void _AlterAssociationHere(long myUserId, long otherUserId, AssociateType? associateType) {
            _OperateOnAssociationsTogether(myUserId, otherUserId, (myAssociates, otherUsersAssociates) => {
                myAssociates.TryGet(otherUserId, out Associate associateOnMe);
                otherUsersAssociates.TryGet(myUserId, out Associate associateOnOtherUser);
                bool iHaveNoAssociate = associateOnMe == null;
                bool theyHaveNoAssociate = associateOnOtherUser == null;
                if (iHaveNoAssociate || theyHaveNoAssociate)
                {
                    //There is no existing association
                    if (!iHaveNoAssociate)
                    {
                        myAssociates.Remove(otherUserId);
                    }
                    if (!theyHaveNoAssociate) {
                        otherUsersAssociates.Remove(myUserId);
                    }
                    _OperationOnRequestsTogether(myUserId, otherUserId, 
                        (mySentRequests, myReceivedRequests, otherUserSendRequests, otherUserReceivedRequests) => {

                            if (associateType == null || (AssociateType)associateType <= 0)
                            {
                                mySentRequests.Remove(otherUserId);
                                myReceivedRequests.Remove(otherUserId);
                                otherUserSendRequests.Remove(myUserId);
                                otherUserReceivedRequests.Remove(myUserId);
                            }
                            mySentRequests.Add(otherUserId, requestUniqueIdentifier, (AssociateType) associateType, false)
                    });
                    return;
                }
                //ignore any existing associations
                //add any overlapping associations
                //request any others
                AssociateType toAdds = 
            });
        }*/
        private void _AcceptRequestHere(long myUserId, long otherUserId, long requestUniqueIdentifier, AssociateType? limitTo,
            out UserProfileSummary myUserProfileSummary, out UserProfileSummary otherUserProfileSummary)
        {
            bool doNotAccept = false;
            AssociateType associateType = AssociateType.None;
            long receivedRequestSentAt = 0;
            if (otherUserId > myUserId)
            {
                DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserReceivedRequests) =>
                {
                    DalAssociateRequests.Instance.ModifySentRequests(otherUserId, (otherUserSentRequests) =>
                    {
                        try
                        {
                            DalAssociateRequests.Instance.ModifyReceivedRequests(myUserId, (myReceivedRequests) =>
                            {
                                DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) =>
                                {
                                    AssociateRequest associateRequest = _ValidateRequestUniqueIdentifierMatches(myReceivedRequests,
                                    myUserId, otherUserId, requestUniqueIdentifier);
                                    associateType = associateRequest.AssociateType;
                                    if (limitTo != null)
                                        associateType = associateType & (AssociateType)limitTo;
                                    doNotAccept = associateType <= AssociateType.None;
                                    if (doNotAccept)
                                        return mySentRequests;
                                    myReceivedRequests.Remove(otherUserId);
                                    mySentRequests.Remove(otherUserId);
                                    otherUserSentRequests.Remove(myUserId);
                                    otherUserReceivedRequests.Remove(myUserId);
                                    receivedRequestSentAt =associateRequest.SentAtUTCMilliseconds;
                                    return mySentRequests;
                                });
                                return myReceivedRequests;
                            });
                        }
                        catch (Exception ex) { Logs.Default.Error(ex); }
                        return otherUserSentRequests;
                    });
                    return otherUserReceivedRequests;
                });
                /*if (associateType == AssociateType.None)
                    throw new ArgumentException($"Cannot add an associate using {nameof(AssociateType)}.{nameof(AssociateType.None)}");*/
            }
            else
            {
                DalAssociateRequests.Instance.ModifyReceivedRequests(myUserId, (myReceivedRequests) =>
                {
                    DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) =>
                    {
                        try
                        {
                            AssociateRequest associateRequest = _ValidateRequestUniqueIdentifierMatches(myReceivedRequests,
                                myUserId, otherUserId, requestUniqueIdentifier);
                            associateType = associateRequest.AssociateType;
                            if (limitTo != null)
                                associateType = associateType & (AssociateType)limitTo;
                            doNotAccept = associateType <= AssociateType.None;
                            if (doNotAccept)
                                return mySentRequests;
                            DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserReceivedRequests) =>
                            {
                                DalAssociateRequests.Instance.ModifySentRequests(otherUserId, (otherUserSentRequests) =>
                                {
                                    otherUserReceivedRequests.Remove(myUserId);
                                    otherUserSentRequests.Remove(myUserId);
                                    myReceivedRequests.Remove(otherUserId);
                                    mySentRequests.Remove(otherUserId);
                                    return otherUserSentRequests;
                                });
                                return otherUserReceivedRequests;
                            });
                            receivedRequestSentAt = associateRequest.SentAtUTCMilliseconds;
                        }
                        catch (Exception ex) { Logs.Default.Error(ex); }
                        return mySentRequests;
                    });
                    return myReceivedRequests;
                });
            }
            AssociateType allAssociateTypes = _AddAssociation(myUserId, otherUserId, associateType);
            myUserProfileSummary = DalUserProfiles.Instance.GetUserProfile(myUserId)
                ?.ToPublicSummary(allAssociateTypes);
            otherUserProfileSummary = DalUserProfiles.Instance.GetUserProfile(otherUserId)
                ?.ToPublicSummary(allAssociateTypes);
            /*if (!doNotAccept) {
                NotificationsCore.UserNotificationsMesh.Instance.ClearUserNotification(myUserId, NotificationType.AssociateRequests, upToAtInclusive: receivedRequestSentAt);
            }*/
        }
        private AssociateRequest _ValidateRequestUniqueIdentifierMatches(AssociateRequests myReceivedRequests,
            long myUserId, long otherUserId, long requestUniqueIdentifier)
        {
            if (!myReceivedRequests.TryGet(otherUserId, out AssociateRequest request))
                throw new Exception($"Had no request from {otherUserId} to {myUserId}");
            if ((request.RequestUniqueIdentifier<=0)&&(request.RequestUniqueIdentifier != requestUniqueIdentifier))
                throw new Exception($"{nameof(requestUniqueIdentifier)} did not match:expected {requestUniqueIdentifier} but found {request.RequestUniqueIdentifier} ");
            return request;
        }
        private AssociateType _AddAssociation(long myUserId, long otherUserId, AssociateType associateType)
        {
            AssociateType allAssociateTypes = AssociateType.None;
            _OperateOnAssociations(myUserId, otherUserId, (associations, userId) =>
            {
                if (associations.TryGet(userId, out Associate associate)) {
                    associate.AssociateType |= associateType;
                    allAssociateTypes = associate.AssociateType;
                    return;
                }
                associate = associations.AddAssociate(userId, associateType);
                allAssociateTypes = associate.AssociateType;
            });
            return allAssociateTypes;
        }
        private void _CancelSentRequestHere(long myUserId, long otherUserId)
        {
            //RULES. ALWAYS LOCK HIGHEST FIRST. This fits in with redistributing load nicer as more servers added.
            if (otherUserId > myUserId)
            {
                DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserReceivedRequests) =>
                {
                    try
                    {
                        DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) =>
                        {
                            mySentRequests.Remove(otherUserId);
                            return mySentRequests;
                        });
                        otherUserReceivedRequests.Remove(myUserId);
                    }
                    catch { }
                    return otherUserReceivedRequests;
                });
                return;
            }
            DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) => {
                try
                {
                    DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserRecievedRequests) =>
                    {
                        otherUserRecievedRequests.Remove(myUserId);
                        return otherUserRecievedRequests;
                    });

                    mySentRequests.Remove(otherUserId);
                }
                catch { }
                return mySentRequests;
            });
        }
        private AssociateType _DowngradeAssociateHere(long myUserId, long otherUserId, AssociateType associationTypesToKeep)
        {
            AssociateType associateTypeRemaining = AssociateType.None; 
            _OperateOnAssociations(myUserId, otherUserId, (associations, userId) =>
            {
                associateTypeRemaining = associations.DowngradeAssociation(userId, associationTypesToKeep);
            });
            return associateTypeRemaining;
        }
        private void _RemoveAssociateHere(long myUserId, long otherUserId)
        {
            _OperateOnAssociations(myUserId, otherUserId, (associations, userId) =>
            {
                associations.Remove(userId);
            });
        }

        private void _OperateOnAssociationsTogether(long myUserId, long otherUserId, Action<Associates, Associates> callbackOperateOnAssociations)
        {


            //RULES. ALWAYS LOCK HIGHEST FIRST. This fits in with redistributing load nicer as more servers added.
            if (otherUserId > myUserId)
            {
                DalAssociates.Instance.ModifyAssociates(otherUserId, (otherUserAssociates) =>
                {
                        DalAssociates.Instance.ModifyAssociates(myUserId, (myAssociates) =>
                        {
                            callbackOperateOnAssociations(myAssociates, otherUserAssociates);
                            return myAssociates;
                        });
                    return otherUserAssociates;
                });
            }
            DalAssociates.Instance.ModifyAssociates(myUserId, (myAssociates) => {
                    DalAssociates.Instance.ModifyAssociates(otherUserId, (otherUserAssociates) =>
                    {
                        callbackOperateOnAssociations(myAssociates, otherUserAssociates);
                        return otherUserAssociates;
                    });
                return myAssociates;
            });
        }

        private void _OperationOnRequestsTogether(long myUserId, long otherUserId, 
            Action<AssociateRequestsSent, AssociateRequestsReceived, AssociateRequestsSent, AssociateRequestsReceived> callback)
        {
            if (otherUserId > myUserId)
            {
                DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserReceivedRequests) =>
                {
                    DalAssociateRequests.Instance.ModifySentRequests(otherUserId, (otherUserSentRequests) =>
                    {
                        try
                        {
                            DalAssociateRequests.Instance.ModifyReceivedRequests(myUserId, (myReceivedRequests) =>
                            {
                                DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) =>
                                {
                                    callback(mySentRequests, myReceivedRequests, otherUserSentRequests, otherUserReceivedRequests);
                                    return mySentRequests;
                                });
                                return myReceivedRequests;
                            });
                        }
                        catch (Exception ex) { Logs.Default.Error(ex); }
                        return otherUserSentRequests;
                    });
                    return otherUserReceivedRequests;
                });
                return;
            }
            DalAssociateRequests.Instance.ModifyReceivedRequests(myUserId, (myReceivedRequests) => {
                DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) => {
                    try
                    {
                        DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserReceivedRequests) =>
                        {
                            DalAssociateRequests.Instance.ModifySentRequests(otherUserId, (otherUserSentRequests) =>
                            {
                                callback(mySentRequests, myReceivedRequests, otherUserSentRequests, otherUserReceivedRequests);
                                return otherUserSentRequests;
                            });
                            return otherUserReceivedRequests;
                        });
                    }
                    catch (Exception ex) { Logs.Default.Error(ex); }
                    return mySentRequests;
                });
                return myReceivedRequests;
            });
        }
        private void _OperateOnAssociations(long myUserId, long otherUserId, Action<Associates, long> callbackOperateOnAssociations)
        {


            //RULES. ALWAYS LOCK HIGHEST FIRST. This fits in with redistributing load nicer as more servers added.
            if (otherUserId > myUserId)
            {
                DalAssociates.Instance.ModifyAssociates(otherUserId, (otherUserAssociates) =>
                {
                    try
                    {
                        DalAssociates.Instance.ModifyAssociates(myUserId, (myAssociates) =>
                        {
                            callbackOperateOnAssociations(myAssociates, otherUserId);
                            return myAssociates;
                        });
                        callbackOperateOnAssociations(otherUserAssociates, myUserId);
                    }
                    catch { }
                    return otherUserAssociates;
                });
                return;
            }
            DalAssociates.Instance.ModifyAssociates(myUserId, (myAssociates) => {
                try
                {
                    DalAssociates.Instance.ModifyAssociates(otherUserId, (otherUserAssociates) =>
                    {
                        callbackOperateOnAssociations(otherUserAssociates, myUserId);
                        return otherUserAssociates;
                    });

                    callbackOperateOnAssociations(myAssociates, otherUserId);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
                return myAssociates;
            });
        }
        private void _RejectAndCounterRequestHere(long myUserId, long otherUserId, AssociateType counterAssociateType) {
            //RULES. ALWAYS LOCK HIGHEST FIRST. This fits in with redistributing load nicer as more servers added.

        }
        private void _RejectRequestHere(long myUserId, long otherUserId)
        {
            //RULES. ALWAYS LOCK HIGHEST FIRST. This fits in with redistributing load nicer as more servers added.
            AssociateRequest associateRequest = null;
            if (otherUserId > myUserId)
            {
                DalAssociateRequests.Instance.ModifySentRequests(otherUserId, (otherUserSentRequests) =>
                {
                    try
                    {
                        DalAssociateRequests.Instance.ModifyReceivedRequests(myUserId, (myReceivedRequests) =>
                        {
                            myReceivedRequests.TryGet(otherUserId, out associateRequest);
                            myReceivedRequests.Remove(otherUserId);
                            return myReceivedRequests;
                        });
                        otherUserSentRequests.Remove(myUserId);
                    }
                    catch { }
                    return otherUserSentRequests;
                });
            }
            else
            {
                DalAssociateRequests.Instance.ModifyReceivedRequests(myUserId, (myReceivedRequests) =>
                {
                    try
                    {
                        DalAssociateRequests.Instance.ModifySentRequests(otherUserId, (otherUserSentRequests) =>
                        {
                            otherUserSentRequests.Remove(myUserId);
                            return otherUserSentRequests;
                        });
                        myReceivedRequests.TryGet(otherUserId, out associateRequest);
                        myReceivedRequests.Remove(otherUserId);
                    }
                    catch { }
                    return myReceivedRequests;
                });
            }
            /*if (associateRequest != null)
            {
                NotificationsCore.UserNotificationsMesh.Instance.ClearUserNotification(
                    myUserId, NotificationType.AssociateRequests, upToAtInclusive: associateRequest.SentAtUTCMilliseconds);
            }*/
        }
        private long _RequestAssociateHere(long myUserId, long otherUserId,
            AssociateType associateType, out AssociateRequestUserProfileSummary actingAssociateRequestUserProfileSummary,
                        out AssociateRequestUserProfileSummary otherUserAssociateRequestUserProfileSummary)
        {
            AssociateRequest myRequestInternal = null;
            AssociateRequest otherUserRequestInternal = null;
            long requestUniqueIdentifier = RequestAssociateUniqueIdentifierSource.Instance.NextId();
            if (otherUserId > myUserId)
            {
                DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserReceivedRequests) =>
                {
                    try
                    {
                        DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) =>
                        {
                            myRequestInternal = mySentRequests.Add(otherUserId, requestUniqueIdentifier, associateType);
                            return mySentRequests;
                        });
                        otherUserRequestInternal = otherUserReceivedRequests.Add(myUserId, requestUniqueIdentifier, associateType);
                    }
                    catch { }
                    return otherUserReceivedRequests;
                });
            }
            else
            {
                DalAssociateRequests.Instance.ModifySentRequests(myUserId, (mySentRequests) =>
                {
                    try
                    {
                        DalAssociateRequests.Instance.ModifyReceivedRequests(otherUserId, (otherUserReceivedRequests) =>
                        {
                            otherUserRequestInternal = otherUserReceivedRequests.Add(myUserId, requestUniqueIdentifier, associateType);
                            return otherUserReceivedRequests;
                        });
                        myRequestInternal = mySentRequests.Add(otherUserId, requestUniqueIdentifier, associateType);
                    }
                    catch { }
                    return mySentRequests;
                });
            }
            //NotificationsCore.UserNotificationsMesh.Instance.SetHasAt(otherUserId, NotificationType.AssociateRequests, otherUserRequestInternal.SentAtUTCMilliseconds);
            UserProfileSummary myUserProfileSummary = DalUserProfiles.Instance.GetUserProfile(myUserId)
                ?.ToPublicSummary(AssociateType.None);
            UserProfileSummary otherUserProfileSummary = DalUserProfiles.Instance.GetUserProfile(otherUserId)
                ?.ToPublicSummary(AssociateType.None );
            actingAssociateRequestUserProfileSummary = new AssociateRequestUserProfileSummary(otherUserRequestInternal, myUserProfileSummary);
            otherUserAssociateRequestUserProfileSummary = new AssociateRequestUserProfileSummary(myRequestInternal, otherUserProfileSummary);
            return requestUniqueIdentifier;
        }
        private Associate _GetAssociate_Here(long myUserId, long otherUserId)
        {
            Associates myAssociates = DalAssociates.Instance.GetUsersAssociates(myUserId);
            if (myAssociates == null) return null;
            if (!myAssociates.TryGet(otherUserId, out Associate associate))
                return null;
            return associate;
        }
        private AssociateRequest _GetAssociateRequestReceived_Here(long onAssociateUserId, long fromAssociateUserId)
        {
            AssociateRequestsReceived associateRequestsReceived = DalAssociateRequests.Instance
                .GetReceivedRequests(onAssociateUserId);
            if (associateRequestsReceived == null) return null;
            if (!associateRequestsReceived.TryGet(fromAssociateUserId, out AssociateRequest associateReceivedRequest))
                return null;
            return associateReceivedRequest;
        }
        private void _ModifyUserProfile_Here(long myUserId, UserProfile userProfileChanges) {

            UserProfile userProfileExternal = null;
            bool usernameChanged = false;
            DalUserProfiles.Instance.ModifyUserProfile(myUserId, (myUserProfile) => {
                //TODO  IF PROFILE DOESNT EXIST? CORRUPTED OR SOMETHING
                string existingUsername = myUserProfile.Username;
                myUserProfile.Update(userProfileChanges);
                usernameChanged = myUserProfile.Username!= existingUsername;
                userProfileExternal = myUserProfile;
                return myUserProfile;
            });
            //TODO ability to delete current location?
            if (userProfileChanges.Location != null) {
                UserQuadTree.Instance.Set(myUserId, userProfileChanges.Location.LatLng);
            }
            if (usernameChanged) {
                DalUserProfiles.Instance.UsernameSearchAddUser(myUserId, userProfileChanges.Username);
            }
            //FrequentlyAccessedUserProfilesManager.Instance.Set(myUserId, userProfileExternal.ToFrequentlyAccessed());
            //_PushUpdateNotificationToClientsOnNewThread(AssociatesOperation.MyProfileModified, myUserId, myUserId);
        }
        private bool UsernameSearchSearch_Here(string str, int maxNEntries, out long[] userIds) {
            try
            {
                userIds = DalUserProfiles.Instance.UsernameSearchSearch(str, maxNEntries);
                return true;
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                userIds = null;
                return false;
            }
        }
        private bool UsernameSearchAddUser_Here(string username, long userId)
        {
            try
            {
                DalUserProfiles.Instance.UsernameSearchAddUser(userId, username);
                return true;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
        private bool UsernameSearchRemoveUser_Here(long userId)
        {
            try
            {
                DalUserProfiles.Instance.UsernameSearchRemoveUser(userId);
                return true;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return false;
            }
        }
    }
}
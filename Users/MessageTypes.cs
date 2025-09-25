using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace Users
{
    public class MessageTypes
    {
        public const string
        UsersGetMyAssociatesProfileSummaries = "ugmaps",
        UsersGetAnotherUsersAssociatesUserProfileSummarys = "ugaups",
        UsersRemoveAssociate = "urea",
        UsersDowngradeAssociation = "uga",
        UsersGetUserProfile = "ugup",
        UsersGetMyUserProfile = "ugmup",
        UsersGetPublicUserProfileSummarys = InterserverMessageTypes.UsersGetPublicUserProfileSummarys,
        UsersModifyUserProfile = InterserverMessageTypes.UserModifyUserProfile,
        UsersGetAllAssociateEntries = InterserverMessageTypes.UsersGetAllAssociateEntries,
        UsersGetMyReceivedRequests = "ugmrr",
        UsersGetMySentRequests = "ugmsr",
        UsersCancelSentRequest = "ucsr",
        UsersRejectRequest = "urr",
        UsersCounterRequest = "ucr",
        UsersAlterAssociate = "ualr",
        UsersAcceptRequest = "uar",
        UsersInviteAssociateByUserId = "uiabui",
        UsersRequestAssociate = "ura",
        UsersAssociateUpdate = InterserverMessageTypes.UsersAssociateUpdate,
        UsernameSearchSearch = InterserverMessageTypes.UsernameSearchSearch;
    }
}
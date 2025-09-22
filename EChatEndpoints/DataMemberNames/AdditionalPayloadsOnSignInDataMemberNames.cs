using MessageTypes.Attributes;
using NotificationsCore.DataMemberNames.Messages;
using Users.DataMemberNames.Responses;

namespace EChat.DataMemberNames.Messages
{
    public static class AdditionalPayloadsOnSignInDataMemberNames
    {
        [DataMemberNamesClass(typeof(UserNotificationsDataMemberNames), isArray:false)]
        public const string UserNotifications = "u";
        [DataMemberNamesClass(typeof(GetAllAssociateEntriesResponseDataMemberNames), isArray: false)]
        public const string AllAssociateEntries = "h";
    }
}
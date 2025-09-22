using MessageTypes.Attributes;

namespace NotificationsCore.DataMemberNames.Messages
{
    public static class UserNotificationsDataMemberNames
    {
        [DataMemberNamesClass(typeof(UserNotificationDataMemberNames), isArray:true)]
        public const string Entries = "e";
    }
}
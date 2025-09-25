using MessageTypes.Attributes;

namespace NotificationsCore.DataMemberNames.Messages
{
    [MessageType(MessageTypes.UserNotificationsUpdate)]
    public static class UserNotificationUpdateDataMemberNames
    {
        public const string NotificationType = "n";
        public const string At = "a";
    }
}
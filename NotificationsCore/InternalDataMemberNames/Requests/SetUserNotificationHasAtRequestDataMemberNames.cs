using MessageTypes.Attributes;

namespace NotificationsCore.InternalDataMemberNames.Requests
{
    public static class SetUserNotificationHasAtRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string UserId = "u";
        public const string NotificationType = "n";
        public const string At = "a";
    }
}
using MessageTypes.Attributes;

namespace NotificationsCore.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UserNotificationsClear)]
    public static class ClearUserNotificationRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON:true)]
        public const string UserId = "u";
        public const string NotificationType = "n";
        public const string UpToAtInclusive = "a";
    }
}
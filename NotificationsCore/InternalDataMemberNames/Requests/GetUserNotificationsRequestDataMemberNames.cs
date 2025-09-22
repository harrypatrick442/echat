using MessageTypes.Attributes;

namespace NotificationsCore.InternalDataMemberNames.Requests
{
    public static class GetUserNotificationsRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON: true)]
        public const string UserId = "u";
    }
}
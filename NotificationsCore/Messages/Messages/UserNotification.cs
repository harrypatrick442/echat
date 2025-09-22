using NotificationsCore.DataMemberNames.Messages;
using NotificationsCore.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace NotificationsCore.Messages.Messages
{
    [DataContract]
    public class UserNotification
    {
        [JsonPropertyName(UserNotificationDataMemberNames.At)]
        [JsonInclude]
        [DataMember(Name = UserNotificationDataMemberNames.At)]
        public long At { get; set; }
        [JsonPropertyName(UserNotificationDataMemberNames.NotificationType)]
        [JsonInclude]
        [DataMember(Name = UserNotificationDataMemberNames.NotificationType)]
        public NotificationType NotificationType { get; set; }
        public UserNotification(NotificationType notificationType, long hasAt)
        {
            NotificationType = notificationType;
            At = hasAt;
        }
        protected UserNotification()
        {

        }
    }
}
using Core.Messages.Messages;
using MessageTypes.Internal;
using NotificationsCore.DataMemberNames.Messages;
using NotificationsCore.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace NotificationsCore.Messages.Messages
{
    [DataContract]
    public class UserNotificationUpdate : TypedMessageBase
    {
        [JsonPropertyName(UserNotificationUpdateDataMemberNames.At)]
        [JsonInclude]
        [DataMember(Name = UserNotificationUpdateDataMemberNames.At)]
        public long? At { get; set; }
        [JsonPropertyName(UserNotificationUpdateDataMemberNames.NotificationType)]
        [JsonInclude]
        [DataMember(Name = UserNotificationUpdateDataMemberNames.NotificationType)]
        public NotificationType NotificationType { get; set; }
        public UserNotificationUpdate(NotificationType notificationType, long? at) : base()
        {
            NotificationType = notificationType;
            At = at;
            _Type = InterserverMessageTypes.UserNotificationsUpdate;
        }
        protected UserNotificationUpdate() : base()
        {
            _Type = InterserverMessageTypes.UserNotificationsUpdate;
        }
    }
}
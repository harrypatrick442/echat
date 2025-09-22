using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MessageTypes.Internal;
using NotificationsCore.Enums;
using NotificationsCore.InternalDataMemberNames.Requests;

namespace NotificationsCore.Messages.Requests
{
    [DataContract]
    public class SetUserNotificationHasAtRequest : TypedMessageBase
    {
        [JsonPropertyName(SetUserNotificationHasAtRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = SetUserNotificationHasAtRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(SetUserNotificationHasAtRequestDataMemberNames.NotificationType)]
        [JsonInclude]
        [DataMember(Name = SetUserNotificationHasAtRequestDataMemberNames.NotificationType)]
        public NotificationType NotificationType { get; protected set; }
        [JsonPropertyName(SetUserNotificationHasAtRequestDataMemberNames.At)]
        [JsonInclude]
        [DataMember(Name = SetUserNotificationHasAtRequestDataMemberNames.At)]
        public long At { get; protected set; }

        protected SetUserNotificationHasAtRequest()
            : base() {
            _Type = InterserverMessageTypes.UserNotificationsSetUserNotificationHasAt;
        }
        public SetUserNotificationHasAtRequest(long userId, NotificationType notificationType, long at) 
            : base()
        {
            _Type = InterserverMessageTypes.UserNotificationsSetUserNotificationHasAt;
            UserId = userId;
            NotificationType = notificationType;
            At = at;
        }
    }
}
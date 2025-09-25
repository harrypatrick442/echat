using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using NotificationsCore.DataMemberNames.Requests;
using NotificationsCore.Enums;

namespace NotificationsCore.Messages.Requests
{
    [DataContract]
    public class ClearUserNotificationRequest : TicketedMessageBase
    {
        [JsonPropertyName(ClearUserNotificationRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = ClearUserNotificationRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(ClearUserNotificationRequestDataMemberNames.NotificationType)]
        [JsonInclude]
        [DataMember(Name = ClearUserNotificationRequestDataMemberNames.NotificationType)]
        public NotificationType NotificationType { get; protected set; }
        [JsonPropertyName(ClearUserNotificationRequestDataMemberNames.UpToAtInclusive)]
        [JsonInclude]
        [DataMember(Name = ClearUserNotificationRequestDataMemberNames.UpToAtInclusive)]
        public long UpToAtInclusive { get; protected set; }
        protected ClearUserNotificationRequest()
            : base(MessageTypes.UserNotificationsClear) { }
        public ClearUserNotificationRequest(long userId, NotificationType notificationType, long upToAtInclusive) 
            : base(MessageTypes.UserNotificationsClear) {
            UserId = userId;
            NotificationType = notificationType;
            UpToAtInclusive = upToAtInclusive;
        }
    }
}

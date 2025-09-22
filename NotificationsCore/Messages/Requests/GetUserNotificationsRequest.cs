using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MessageTypes.Internal;
using NotificationsCore.InternalDataMemberNames.Requests;

namespace NotificationsCore.Messages.Requests
{
    [DataContract]
    public class GetUserNotificationsRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetUserNotificationsRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = GetUserNotificationsRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        
        protected GetUserNotificationsRequest()
            : base(InterserverMessageTypes.UserNotificationsGetUserNotifications) { }
        public GetUserNotificationsRequest(long userId) 
            : base(InterserverMessageTypes.UserNotificationsGetUserNotifications) {
            UserId = userId;
        }
    }
}

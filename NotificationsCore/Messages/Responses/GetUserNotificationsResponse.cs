using Core.DataMemberNames;
using System.Runtime.Serialization;
using Core.Messages.Messages;
using NotificationsCore.Messages.Messages;
using System.Text.Json.Serialization;
using NotificationsCore.InternalDataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetUserNotificationsResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetUserNotificationsResponseDataMemberNames.UserNotifications)]
        [JsonInclude]
        [DataMember(Name = GetUserNotificationsResponseDataMemberNames.UserNotifications)]
        public UserNotifications UserNotifications{ get; protected set; }
        public GetUserNotificationsResponse(UserNotifications userNotifications, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            UserNotifications = userNotifications;
            _Ticket = ticket;
        }
        protected GetUserNotificationsResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

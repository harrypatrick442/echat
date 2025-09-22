using Core.DataMemberNames;
using System.Runtime.Serialization;
using Core.Messages.Messages;
using System.Text.Json.Serialization;
using NotificationsCore.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class ClearUserNotificationResponse : TicketedMessageBase
    {
        [JsonPropertyName(ClearUserNotificationResponseDataMemberNames.Cleared)]
        [JsonInclude]
        [DataMember(Name = ClearUserNotificationResponseDataMemberNames.Cleared)]
        public bool Cleared { get; protected set; }
        public ClearUserNotificationResponse(bool cleared, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Cleared = cleared;
            _Ticket = ticket;
        }
        protected ClearUserNotificationResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

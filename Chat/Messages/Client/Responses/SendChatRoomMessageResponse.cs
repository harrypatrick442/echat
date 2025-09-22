using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public sealed class SendChatRoomMessageResponse : TicketedMessageBase
    {
        [JsonPropertyName(SendChatRoomMessageResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = SendChatRoomMessageResponseDataMemberNames.FailedReason)]
        public ChatFailedReason? FailedReason
        {
            get;
            protected set;
        }
        private SendChatRoomMessageResponse(ChatFailedReason? failedReason,
            long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected SendChatRoomMessageResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static SendChatRoomMessageResponse Successful(long ticket)
        {
            return new SendChatRoomMessageResponse(null, ticket);
        }
        public static SendChatRoomMessageResponse Failed(ChatFailedReason failedReason, long ticket)
        {
            return new SendChatRoomMessageResponse(failedReason, ticket);
        }
    }
}

using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.Messages.Client.Messages;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class SendMessageResponse : TicketedMessageBase
    {
        private bool _Successful;
        [JsonPropertyName(SendMessageResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = SendMessageResponseDataMemberNames.Successful)]
        public bool Successful
        {
            get { return _Successful; }
            protected set { _Successful = value; }
        }
        private ChatFailedReason _FailedReason;
        [JsonPropertyName(SendMessageResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = SendMessageResponseDataMemberNames.FailedReason)]
        public ChatFailedReason FailedReason
        {
            get { return _FailedReason; }
            protected set { _FailedReason = value; }
        }
        [JsonPropertyName(SendMessageResponseDataMemberNames.ReplyMessage)]
        [JsonInclude]
        [DataMember(Name = SendMessageResponseDataMemberNames.ReplyMessage)]
        public ClientMessage ReplyMessage
        {
            get;
            protected set;
        }
        public SendMessageResponse(bool successful, ChatFailedReason failedReason, ClientMessage replyMessage,
            long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            _Ticket = ticket;
            _Successful = successful;
            _FailedReason = failedReason;
            ReplyMessage = replyMessage;
        }
        protected SendMessageResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static SendMessageResponse Failed(ChatFailedReason failedReason, long ticket)
        {
            return new SendMessageResponse(false, failedReason, null, ticket);
        }
        public static SendMessageResponse Success(ClientMessage replyMessage, long ticket)
        {
            return new SendMessageResponse(true, ChatFailedReason.None, replyMessage, ticket);
        }
    }
}

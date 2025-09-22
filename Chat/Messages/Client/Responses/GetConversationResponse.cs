using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using DataMemberNames.Client.Chat.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetConversationResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetConversationResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetConversationResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(GetConversationResponseDataMemberNames.Conversation)]
        [JsonInclude]
        [DataMember(Name = GetConversationResponseDataMemberNames.Conversation)]
        public Conversation Conversation { get; protected set; }
        [JsonPropertyName(GetConversationResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = GetConversationResponseDataMemberNames.FailedReason)]
        public ChatFailedReason FailedReason
        {
            get;
            protected set;
        }
        protected GetConversationResponse(bool successful, Conversation conversation,
            ChatFailedReason failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Conversation = conversation;
            FailedReason = failedReason;
            _Ticket = ticket;
        }
        protected GetConversationResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetConversationResponse Success(Conversation conversation,
            ChatFailedReason failedReason, long ticket)
        {
            return new GetConversationResponse(true, conversation, failedReason, ticket);
        }
        public static GetConversationResponse Failed(ChatFailedReason failedReason, long ticket)
        {
            return new GetConversationResponse(false, null, failedReason, ticket);
        }
    }
}

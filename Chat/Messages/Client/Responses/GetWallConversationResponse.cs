using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using DataMemberNames.Client.Chat.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetWallConversationResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetWallConversationResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetWallConversationResponseDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(GetWallConversationResponseDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = GetWallConversationResponseDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(GetWallConversationResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = GetWallConversationResponseDataMemberNames.FailedReason)]
        public ChatFailedReason FailedReason
        {
            get;
            protected set;
        }
        public GetWallConversationResponse(bool successful, long conversationId,
            ChatFailedReason failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            ConversationId = conversationId;
            FailedReason = failedReason;
            _Ticket = ticket;
        }
        protected GetWallConversationResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

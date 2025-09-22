using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MultimediaCore;
using MultimediaServerCore.Enums;
using Chat.DataMemberNames.Responses;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetConversationIdResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetConversationIdResponseDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = GetConversationIdResponseDataMemberNames.ConversationId)]
        public long ConversationId{ get; protected set; }
        [JsonPropertyName(GetConversationIdResponseDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = GetConversationIdResponseDataMemberNames.Successful)]
        public bool Successful
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetConversationIdResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = GetConversationIdResponseDataMemberNames.FailedReason)]
        public ChatFailedReason FailedReason
        {
            get;
            protected set;
        }
        public GetConversationIdResponse(bool successful, long conversationId, 
            ChatFailedReason failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            ConversationId = conversationId;
            FailedReason = failedReason;
            _Ticket = ticket;
        }
        protected GetConversationIdResponse()
            : base(TicketedMessageType.Ticketed) { }
        public static GetConversationIdResponse Failed(ChatFailedReason failedReason, long ticket) {
            return new GetConversationIdResponse(false, -1, failedReason, ticket);
        }
    }
}

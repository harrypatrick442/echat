using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using DataMemberNames.Client.Chat.Requests;
using DataMemberNames.Client.Users;
using DataMemberNames.Client;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetConversationRequest : TicketedMessageBase
    {
        private long _ConversationId;
        [JsonPropertyName(GetConversationRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = GetConversationRequestDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get { return _ConversationId; }
            protected set { _ConversationId = value; }
        }
        public GetConversationRequest(long conversationId)
            : base(ClientMessageTypes.ChatGetConversation)
        {
            _ConversationId = conversationId;
        }
        protected GetConversationRequest()
            : base(ClientMessageTypes.ChatGetConversation) { }
    }
}

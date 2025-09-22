using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using DataMemberNames.Client;
using DataMemberNames.Client.Chat.Requests;

namespace Chat.Messages.Interserver
{
    [DataContract]
    public class GetConversationInterserverRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(SendMessageRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = SendMessageRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _ConversationId;
        [JsonPropertyName(SendMessageRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = SendMessageRequestDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get { return _ConversationId; }
            protected set { _ConversationId = value; }
        }
        public GetConversationInterserverRequest(long myUserId, long conversationId)
            : base(ClientMessageTypes.ChatGetConversation)
        {
            _MyUserId = myUserId;
            _ConversationId = conversationId;
        }
        protected GetConversationInterserverRequest()
            : base(ClientMessageTypes.ChatGetConversation) { }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.DTOs;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetAdministratorsRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetAdministratorsRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = GetAdministratorsRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(GetAdministratorsRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetAdministratorsRequestDataMemberNames.MyUserId)]
        public long MyUserId { get; protected set; }
        public GetAdministratorsRequest(long conversationId, long myUserId)
            : base(global::MessageTypes.MessageTypes.ChatGetAdministrators)
        {
            ConversationId = conversationId;
            MyUserId = myUserId;
        }
        protected GetAdministratorsRequest()
            : base(global::MessageTypes.MessageTypes.ChatGetAdministrators) { }
    }
}

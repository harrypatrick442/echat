using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetPmConversationInterserverRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(GetPmConversationWithLatestMessagesInterserverRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetPmConversationWithLatestMessagesInterserverRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(GetPmConversationWithLatestMessagesInterserverRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = GetPmConversationWithLatestMessagesInterserverRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        public GetPmConversationInterserverRequest(long myUserId, long otherUserId)
            : base(InterserverMessageTypes.ChatGetPmConversation)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
        }
        protected GetPmConversationInterserverRequest()
            : base(InterserverMessageTypes.ChatGetPmConversation) { }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;
using Users.DataMemberNames.Requests;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetPmConversationRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetPmConversationRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = GetPmConversationRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get;
            protected set;
        }
        public GetPmConversationRequest(long otherUserId)
            : base(InterserverMessageTypes.ChatGetPmConversation)
        {
            OtherUserId = otherUserId;
        }
        protected GetPmConversationRequest()
            : base(InterserverMessageTypes.ChatGetPmConversation) { }
    }
}

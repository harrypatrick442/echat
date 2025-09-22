using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetWallConversationRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetWallConversationRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = GetWallConversationRequestDataMemberNames.UserId)]
        public long UserId
        {
            get;
            protected set;
        }
        [DataMemberNamesIgnore]
        [JsonPropertyName(GetWallConversationRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetWallConversationRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get;
            protected set;
        }
        public GetWallConversationRequest(long myUserId, long userId)
            : base(InterserverMessageTypes.ChatGetWallConversation)
        {
            MyUserId = myUserId;
            UserId = userId;
        }
        protected GetWallConversationRequest()
            : base(InterserverMessageTypes.ChatGetWallConversation) { }
    }
}

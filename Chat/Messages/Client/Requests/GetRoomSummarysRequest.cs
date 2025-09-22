using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetRoomSummarysRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetRoomSummarysRequestDataMemberNames.ConversationIds)]
        [JsonInclude]
        [DataMember(Name =GetRoomSummarysRequestDataMemberNames.ConversationIds)]
        public long[] ConversationIds { get; protected set; }
        public GetRoomSummarysRequest(long[] conversatoinIds)
            : base(global::MessageTypes.MessageTypes.ChatGetRoomSummarys)
        {
            ConversationIds = conversatoinIds;
        }
        protected GetRoomSummarysRequest()
            : base(global::MessageTypes.MessageTypes.ChatGetRoomSummarys) { }
    }
}

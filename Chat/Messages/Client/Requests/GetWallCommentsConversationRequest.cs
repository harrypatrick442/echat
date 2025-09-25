using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetWallCommentsConversationRequest : TicketedMessageBase
    {
        [DataMemberNamesIgnore(toJSON:true)]
        [JsonPropertyName(GetWallCommentsConversationRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetWallCommentsConversationRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetWallCommentsConversationRequestDataMemberNames.WallConversationId)]
        [JsonInclude]
        [DataMember(Name = GetWallCommentsConversationRequestDataMemberNames.WallConversationId)]
        public long WallConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(GetWallCommentsConversationRequestDataMemberNames.WallMessageId)]
        [JsonInclude]
        [DataMember(Name = GetWallCommentsConversationRequestDataMemberNames.WallMessageId)]
        public long WallMessageId
        {
            get;
            protected set;
        }
        public GetWallCommentsConversationRequest(
            long myUserId,
            long wallConversationId,
            long wallMessageId)
            : base(MessageTypes.ChatGetWallCommentsConversation)
        {
            MyUserId = myUserId;
            WallConversationId = wallConversationId;
            WallMessageId = wallMessageId;
        }
        protected GetWallCommentsConversationRequest()
            : base(InterserverMessageTypes.ChatGetWallCommentsConversation) { }
    }
}

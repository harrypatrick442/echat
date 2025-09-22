using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class AcceptRoomInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(AcceptRoomInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = AcceptRoomInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(AcceptRoomInviteRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = AcceptRoomInviteRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public AcceptRoomInviteRequest(long conversationId, long userId)
            : base(global::MessageTypes.MessageTypes.ChatAcceptRoomInvite)
        {
            ConversationId = conversationId;
            UserId = userId;
        }
        protected AcceptRoomInviteRequest()
            : base(global::MessageTypes.MessageTypes.ChatAcceptRoomInvite) { }
    }
}

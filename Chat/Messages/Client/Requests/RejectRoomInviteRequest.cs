using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RejectRoomInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(RejectRoomInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RejectRoomInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        public RejectRoomInviteRequest(long conversationId)
            : base(MessageTypes.ChatRejectRoomInvite)
        {
            ConversationId = conversationId;
        }
        protected RejectRoomInviteRequest()
            : base(MessageTypes.ChatRejectRoomInvite) { }
    }
}

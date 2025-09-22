using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class CancelRoomInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(CancelRoomInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = CancelRoomInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(CancelRoomInviteRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = CancelRoomInviteRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public CancelRoomInviteRequest(long conversationId, long userId)
            : base(global::MessageTypes.MessageTypes.ChatCancelRoomInvite)
        {
            ConversationId = conversationId;
            UserId = userId;
        }
        protected CancelRoomInviteRequest()
            : base(global::MessageTypes.MessageTypes.ChatCancelRoomInvite) { }
    }
}

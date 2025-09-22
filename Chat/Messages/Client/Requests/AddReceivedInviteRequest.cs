using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class AddReceivedInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(AddReceivedInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = AddReceivedInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(AddReceivedInviteRequestDataMemberNames.UserIdBeingInvited)]
        [JsonInclude]
        [DataMember(Name = AddReceivedInviteRequestDataMemberNames.UserIdBeingInvited)]
        public long UserIdBeingInvited { get; protected set; }
        [JsonPropertyName(AddReceivedInviteRequestDataMemberNames.UserIdInviting)]
        [JsonInclude]
        [DataMember(Name = AddReceivedInviteRequestDataMemberNames.UserIdInviting)]
        public long UserIdInviting { get; protected set; }
        public AddReceivedInviteRequest(long conversationId, long userIdBeingInvited, long userIdInviting)
            : base(InterserverMessageTypes.ChatAddReceivedInvite)
        {
            ConversationId = conversationId;
            UserIdBeingInvited = userIdBeingInvited;
            UserIdInviting = userIdInviting;
        }
        protected AddReceivedInviteRequest()
            : base(InterserverMessageTypes.ChatAddReceivedInvite) { }
    }
}

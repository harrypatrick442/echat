using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class AddSentInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(AddSentInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = AddSentInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(AddSentInviteRequestDataMemberNames.UserIdBeingInvited)]
        [JsonInclude]
        [DataMember(Name = AddSentInviteRequestDataMemberNames.UserIdBeingInvited)]
        public long UserIdBeingInvited { get; protected set; }
        [JsonPropertyName(AddSentInviteRequestDataMemberNames.UserIdInviting)]
        [JsonInclude]
        [DataMember(Name = AddSentInviteRequestDataMemberNames.UserIdInviting)]
        public long UserIdInviting { get; protected set; }
        public AddSentInviteRequest(long conversationId, long userIdBeingInvited, long userIdInviting)
            : base(InterserverMessageTypes.ChatAddSentInvite)
        {
            ConversationId = conversationId;
            UserIdBeingInvited = userIdBeingInvited;
            UserIdInviting = userIdInviting;
        }
        protected AddSentInviteRequest()
            : base(InterserverMessageTypes.ChatAddSentInvite) { }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RemoveSentInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(RemoveSentInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RemoveSentInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(RemoveSentInviteRequestDataMemberNames.UserIdBeingInvited)]
        [JsonInclude]
        [DataMember(Name = RemoveSentInviteRequestDataMemberNames.UserIdBeingInvited)]
        public long UserIdBeingInvited { get; protected set; }
        [JsonPropertyName(RemoveSentInviteRequestDataMemberNames.UserIdInviting)]
        [JsonInclude]
        [DataMember(Name = RemoveSentInviteRequestDataMemberNames.UserIdInviting)]
        public long UserIdInviting { get; protected set; }
        public RemoveSentInviteRequest(long conversationId, long userIdBeingInvited, long userIdInviting)
            : base(InterserverMessageTypes.ChatRemoveSentInvite)
        {
            ConversationId = conversationId;
            UserIdBeingInvited = userIdBeingInvited;
            UserIdInviting = userIdInviting;
        }
        protected RemoveSentInviteRequest()
            : base(InterserverMessageTypes.ChatRemoveSentInvite) { }
    }
}

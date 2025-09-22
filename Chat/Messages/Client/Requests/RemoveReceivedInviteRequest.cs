using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RemoveReceivedInviteRequest : TicketedMessageBase
    {
        [JsonPropertyName(RemoveReceivedInviteRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RemoveReceivedInviteRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(RemoveReceivedInviteRequestDataMemberNames.UserIdBeingInvited)]
        [JsonInclude]
        [DataMember(Name = RemoveReceivedInviteRequestDataMemberNames.UserIdBeingInvited)]
        public long UserIdBeingInvited { get; protected set; }
        [JsonPropertyName(RemoveReceivedInviteRequestDataMemberNames.UserIdInviting)]
        [JsonInclude]
        [DataMember(Name = RemoveReceivedInviteRequestDataMemberNames.UserIdInviting)]
        public long? UserIdInviting { get; protected set; }
        public RemoveReceivedInviteRequest(long conversationId, long userIdBeingInvited, long? userIdInviting)
            : base(InterserverMessageTypes.ChatRemoveReceivedInvite)
        {
            ConversationId = conversationId;
            UserIdBeingInvited = userIdBeingInvited;
            UserIdInviting = userIdInviting;
        }
        protected RemoveReceivedInviteRequest()
            : base(InterserverMessageTypes.ChatRemoveReceivedInvite) { }
    }
}

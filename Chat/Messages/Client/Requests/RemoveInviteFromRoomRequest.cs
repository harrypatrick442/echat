using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class RemoveInviteFromRoomRequest : TicketedMessageBase
    {
        [JsonPropertyName(RemoveInviteFromRoomRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RemoveInviteFromRoomRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(RemoveInviteFromRoomRequestDataMemberNames.UserIdBeingInvited)]
        [JsonInclude]
        [DataMember(Name = RemoveInviteFromRoomRequestDataMemberNames.UserIdBeingInvited)]
        public long UserIdBeingInvited { get; protected set; }
        [JsonPropertyName(RemoveInviteFromRoomRequestDataMemberNames.UserIdInviting)]
        [JsonInclude]
        [DataMember(Name = RemoveInviteFromRoomRequestDataMemberNames.UserIdInviting)]
        public long? UserIdInviting { get; protected set; }
        public RemoveInviteFromRoomRequest(long conversationId, long userIdBeingInvited, long? userIdInviting)
            : base(InterserverMessageTypes.ChatRemoveInviteFromRoom)
        {
            ConversationId = conversationId;
            UserIdBeingInvited = userIdBeingInvited;
            UserIdInviting = userIdInviting;
        }
        protected RemoveInviteFromRoomRequest()
            : base(InterserverMessageTypes.ChatRemoveInviteFromRoom) { }
    }
}

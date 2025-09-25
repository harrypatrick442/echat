using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class RemoveRoomInvite : TypedMessageBase
    {
        [JsonPropertyName(RemoveRoomInviteDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RemoveRoomInviteDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(RemoveRoomInviteDataMemberNames.UserIdBeingInvited)]
        [JsonInclude]
        [DataMember(Name = RemoveRoomInviteDataMemberNames.UserIdBeingInvited)]
        public long UserIdBeingInvited { get; protected set; }
        [JsonPropertyName(RemoveRoomInviteDataMemberNames.UserIdInviting)]
        [JsonInclude]
        [DataMember(Name = RemoveRoomInviteDataMemberNames.UserIdInviting)]
        public long UserIdInviting { get; protected set; }
        public RemoveRoomInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            ConversationId = conversationId;
            UserIdBeingInvited = userIdBeingInvited;
            UserIdInviting = userIdInviting;
            _Type = MessageTypes.ChatRemoveRoomInvite;
        }
    }
}

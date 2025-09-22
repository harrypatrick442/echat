using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class AddRoomInvite : TypedMessageBase
    {
        [JsonPropertyName(AddRoomInviteDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = AddRoomInviteDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(AddRoomInviteDataMemberNames.UserIdBeingInvited)]
        [JsonInclude]
        [DataMember(Name = AddRoomInviteDataMemberNames.UserIdBeingInvited)]
        public long UserIdBeingInvited { get; protected set; }
        [JsonPropertyName(AddRoomInviteDataMemberNames.UserIdInviting)]
        [JsonInclude]
        [DataMember(Name = AddRoomInviteDataMemberNames.UserIdInviting)]
        public long UserIdInviting { get; protected set; }
        public AddRoomInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            ConversationId = conversationId;
            UserIdBeingInvited = userIdBeingInvited;
            UserIdInviting = userIdInviting;
            _Type = global::MessageTypes.MessageTypes.ChatAddRoomInvite;
        }
    }
}

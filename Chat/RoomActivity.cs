using Chat.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat
{
    [DataContract]
    public class RoomActivity
    {
        [JsonPropertyName(RoomActivityDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RoomActivityDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(RoomActivityDataMemberNames.NUsers)]
        [JsonInclude]
        [DataMember(Name = RoomActivityDataMemberNames.NUsers)]
        public int NUsers { get; set; }
        [JsonPropertyName(RoomActivityDataMemberNames.Visibility)]
        [JsonInclude]
        [DataMember(Name = RoomActivityDataMemberNames.Visibility)]
        public RoomVisibility Visibility{ get; protected set; }

        public RoomActivity(long conversationId, int nUsers,
            RoomVisibility visibility) { 
            ConversationId = conversationId;
            NUsers = nUsers;
            Visibility = visibility;
        }
        protected RoomActivity() { }
    }
}

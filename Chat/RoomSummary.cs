using Chat.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat
{
    [DataContract]
    public class RoomSummary
    {
        [JsonPropertyName(RoomSnapshotDataMemberNames.Name)]
        [JsonInclude]
        [DataMember(Name =RoomSnapshotDataMemberNames.Name)]
        public string Name { get; protected set; }
        [JsonPropertyName(RoomSnapshotDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = RoomSnapshotDataMemberNames.ConversationId)]
        public long ConversationId { get; set; }
        [JsonPropertyName(RoomSnapshotDataMemberNames.NUsers)]
        [JsonInclude]
        [DataMember(Name = RoomSnapshotDataMemberNames.NUsers)]
        public int NUsers { get; set; }
        [JsonPropertyName(RoomSnapshotDataMemberNames.MainPicture)]
        [JsonInclude]
        [DataMember(Name = RoomSnapshotDataMemberNames.MainPicture)]
        public string MainPicture { get; set; }

        public RoomSummary(long conversationId, string name, 
            int nUsers, string mainPicture) { 
            ConversationId = conversationId;
            Name = name;
            NUsers = nUsers;
            MainPicture = mainPicture;
        }
        protected RoomSummary() { }
    }
}

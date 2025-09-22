using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class ModifyUserRooms : TypedMessageBase
    {
        [JsonPropertyName(ModifyUserRoomsDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ModifyUserRoomsDataMemberNames.ConversationId)]
        public long ConversationId { get; set; }
        [JsonPropertyName(ModifyUserRoomsDataMemberNames.AddElseRemove)]
        [JsonInclude]
        [DataMember(Name = ModifyUserRoomsDataMemberNames.AddElseRemove)]
        public bool AddElseRemove { get; set; }
        [JsonPropertyName(ModifyUserRoomsDataMemberNames.Operations)]
        [JsonInclude]
        [DataMember(Name = ModifyUserRoomsDataMemberNames.Operations)]
        public UserRoomsOperation[] Operations { get; set; }
        public ModifyUserRooms(long conversationId, bool addElseRemove, params UserRoomsOperation[] operations)
            : base()
        {
            Type = global::MessageTypes.MessageTypes.ChatModifyUserRooms;
            ConversationId = conversationId;
            AddElseRemove = addElseRemove;
            Operations = operations;
        }
        protected ModifyUserRooms() { }
    }
}

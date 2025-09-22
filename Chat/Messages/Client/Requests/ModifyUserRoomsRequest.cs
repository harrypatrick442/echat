using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class ModifyUserRoomsRequest : TicketedMessageBase
    {
        [JsonPropertyName(ModifyUserRoomsRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = ModifyUserRoomsRequestDataMemberNames.MyUserId)]
        public long MyUserId { get; set; }
        [JsonPropertyName(ModifyUserRoomsRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ModifyUserRoomsRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; set; }
        [JsonPropertyName(ModifyUserRoomsRequestDataMemberNames.AddElseRemove)]
        [JsonInclude]
        [DataMember(Name = ModifyUserRoomsRequestDataMemberNames.AddElseRemove)]
        public bool AddElseRemove { get; set; }
        [JsonPropertyName(ModifyUserRoomsRequestDataMemberNames.Operations)]
        [JsonInclude]
        [DataMember(Name = ModifyUserRoomsRequestDataMemberNames.Operations)]
        public UserRoomsOperation[] Operations { get; set; }
        public ModifyUserRoomsRequest(long myUserId, long conversationId, bool addElseRemove, UserRoomsOperation[] operations)
            : base(global::MessageTypes.MessageTypes.ChatModifyUserRooms)
        {
            MyUserId = myUserId;
            ConversationId = conversationId;
            AddElseRemove = addElseRemove;
            Operations = operations;
        }
        protected ModifyUserRoomsRequest()
            : base(global::MessageTypes.MessageTypes.ChatModifyUserRooms) { }
    }
}

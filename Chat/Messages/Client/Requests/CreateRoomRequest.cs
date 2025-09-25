using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class CreateRoomRequest : TicketedMessageBase
    {
        [JsonPropertyName(CreateRoomRequestDataMemberNames.Name)]
        [JsonInclude]
        [DataMember(Name = CreateRoomRequestDataMemberNames.Name)]
        public string Name { get; protected set; }
        [JsonPropertyName(CreateRoomRequestDataMemberNames.Visibility)]
        [JsonInclude]
        [DataMember(Name = CreateRoomRequestDataMemberNames.Visibility)]
        public RoomVisibility? Visibility { get; protected set; }
        public CreateRoomRequest(string name)
            : base(MessageTypes.ChatCreateRoom)
        {
            Name = name;
        }
        protected CreateRoomRequest()
            : base(MessageTypes.ChatCreateRoom) { }
    }
}

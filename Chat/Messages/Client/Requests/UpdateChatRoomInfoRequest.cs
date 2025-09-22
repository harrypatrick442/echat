using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class UpdateChatRoomInfoRequest : TicketedMessageBase
    {
        [JsonPropertyName(UpdateChatRoomInfoRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UpdateChatRoomInfoRequestDataMemberNames.UserId)]
        public long UserId { get; set; }
        private string _Name;
        [JsonPropertyName(UpdateChatRoomInfoRequestDataMemberNames.Name)]
        [JsonInclude]
        [DataMember(Name = UpdateChatRoomInfoRequestDataMemberNames.Name)]
        public string Name { 
            get { 
                return _Name;
            } 
            protected set { 
                _Name = value;
                NameChanged = true;
            } 
        }
        public bool NameChanged { get; protected set; }
        public RoomVisibility _Visibility;
        [JsonPropertyName(UpdateChatRoomInfoRequestDataMemberNames.Visibility)]
        [JsonInclude]
        [DataMember(Name = UpdateChatRoomInfoRequestDataMemberNames.Visibility)]
        public RoomVisibility Visibility { 
            get 
            { 
                return _Visibility;
            }
            protected set
            { 
                _Visibility = value;
                VisibilityChanged = true;
            } 
        }
        public bool VisibilityChanged { get; protected set; }
        private string[] _Tags;
        [JsonPropertyName(UpdateChatRoomInfoRequestDataMemberNames.Tags)]
        [JsonInclude]
        [DataMember(Name = UpdateChatRoomInfoRequestDataMemberNames.Tags)]
        public string[] Tags { 
            get {
                return _Tags;
            } 
            protected set {
                _Tags = value;
                TagsChanged = true;
            } 
        }
        public bool TagsChanged { get; protected set; }
        public UpdateChatRoomInfoRequest()
            : base(global::MessageTypes.MessageTypes.ChatUpdateRoomInfo)
        {

        }
    }
}

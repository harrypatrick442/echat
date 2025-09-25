using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.DataMemberNames.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class UpdateToChatRoomInfo : TypedMessageBase
    {
        [JsonPropertyName(UpdateToChatRoomInfoDataMemberNames.Info)]
        [JsonInclude]
        [DataMember(Name = UpdateToChatRoomInfoDataMemberNames.Info)]
        public ChatRoomInfo Info { get; protected set; }
        public UpdateToChatRoomInfo(ChatRoomInfo info)
            : base()
        {
            Type = MessageTypes.ChatUpdateRoomInfo;
            Info = info;
        }
        protected UpdateToChatRoomInfo() { }
    }
}

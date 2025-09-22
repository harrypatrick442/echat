using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class AttemptEnterRoomMessage : TypedMessageBase
    {
        [JsonPropertyName(AttemptEnterRoomMessageDataMemberNames.Password)]
        [JsonInclude]
        [DataMember(Name = AttemptEnterRoomMessageDataMemberNames.Password)]
        public string Password { get; protected set; }
        public AttemptEnterRoomMessage()
        {
            _Type = global::MessageTypes.MessageTypes.ChatAttemptEnterRoom;
        }
    }
}

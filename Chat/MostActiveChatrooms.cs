using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat
{
    [DataContract]
    public class MostActiveChatrooms:TypedMessageBase
    {
        [JsonPropertyName(MostActiveChatroomsDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name =MostActiveChatroomsDataMemberNames.Entries)]
        public RoomActivity[] Entries { get; protected set; }
        public MostActiveChatrooms(RoomActivity[] entries) {
            Type = InterserverMessageTypes.ChatMostActiveRooms;
            Entries = entries;
        }
        protected MostActiveChatrooms() { }
    }
}

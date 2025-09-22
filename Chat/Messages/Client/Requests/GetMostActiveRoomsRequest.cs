using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MessageTypes.Internal;
using Chat.DataMemberNames.Requests;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetMostActiveRoomsRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetMostActiveRoomsRequestDataMemberNames.NMostActive)]
        [JsonInclude]
        [DataMember(Name = GetMostActiveRoomsRequestDataMemberNames.NMostActive)]
        public int NMostActive { get; protected set; }
        public GetMostActiveRoomsRequest(int nMostActive)
            : base(InterserverMessageTypes.ChatGetMostActiveRooms)
        {
            NMostActive = nMostActive;
        }
        protected GetMostActiveRoomsRequest()
            : base(InterserverMessageTypes.ChatGetMostActiveRooms) { }
    }
}

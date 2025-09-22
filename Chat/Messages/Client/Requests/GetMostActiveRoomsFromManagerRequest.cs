using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;
using MessageTypes.Internal;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetMostActiveRoomsFromManagerRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetMostActiveRoomsRequestDataMemberNames.NMostActive)]
        [JsonInclude]
        [DataMember(Name = GetMostActiveRoomsRequestDataMemberNames.NMostActive)]
        public int NMostActive { get; protected set; }
        public GetMostActiveRoomsFromManagerRequest()
            : base(InterserverMessageTypes.ChatGetMostActiveRoomsFromManager)
        {

        }
    }
}

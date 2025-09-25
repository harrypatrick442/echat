using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class SearchRoomsRequest : TicketedMessageBase
    {
        [JsonPropertyName(SearchRoomsRequestDataMemberNames.Str)]
        [JsonInclude]
        [DataMember(Name = SearchRoomsRequestDataMemberNames.Str)]
        public string Str { get; protected set; }
        public SearchRoomsRequest(string str)
            : base(MessageTypes.ChatSearchRooms)
        {
            Str = str;
        }
        protected SearchRoomsRequest()
            : base(MessageTypes.ChatSearchRooms) { }
    }
}

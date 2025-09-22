using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Chat.Messages.Client.Messages;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class SearchRoomsResponse : TicketedMessageBase
    {
        [JsonPropertyName(SearchRoomsResponseDataMemberNames.ConversationWithTagss)]
        [JsonInclude]
        [DataMember(Name = SearchRoomsResponseDataMemberNames.ConversationWithTagss)]
        public ConversationWithTags[]? ConversationWithTagss{ get; protected set; }
        public SearchRoomsResponse(ConversationWithTags[]? conversationWithTagss, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            ConversationWithTagss = conversationWithTagss;
            Ticket = ticket;
        }
        protected SearchRoomsResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BaseMessages.Constants;
using Chat.Messages.Client.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class SendMessageRequest : ClientMessage, ITicketedMessageBase
    {
        protected string _Ticket;
        [JsonPropertyName(Ticketing.TICKET)]
        [JsonInclude]
        [DataMember(Name = Ticketing.TICKET)]
        public long Ticket { get; set; }
        protected SendMessageRequest() { 
        
        }
    }
}

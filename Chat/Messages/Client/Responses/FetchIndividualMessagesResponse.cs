using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Chat.Messages.Client.Messages;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class FetchIndividualMessagesResponse : TicketedMessageBase
    {
        [JsonPropertyName(FetchIndividualMessagesResponseDataMemberNames.Results)]
        [JsonInclude]
        [DataMember(Name = FetchIndividualMessagesResponseDataMemberNames.Results)]
        public FetchConversationIndividualMessagesResult[] Results { get; protected set; }
        public FetchIndividualMessagesResponse(FetchConversationIndividualMessagesResult[] results, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Results = results;
            Ticket = ticket;
        }
        protected FetchIndividualMessagesResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

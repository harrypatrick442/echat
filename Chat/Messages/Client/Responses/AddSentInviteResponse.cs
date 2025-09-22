using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class AddSentInviteResponse : TicketedMessageBase
    {
        [JsonPropertyName(AddSentInviteResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = AddSentInviteResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public AddSentInviteResponse(bool success, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
        protected AddSentInviteResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

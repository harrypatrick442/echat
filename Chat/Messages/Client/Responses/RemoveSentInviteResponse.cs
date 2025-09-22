using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class RemoveSentInviteResponse : TicketedMessageBase
    {
        [JsonPropertyName(RemoveSentInviteResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = RemoveSentInviteResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public RemoveSentInviteResponse(bool success, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
        protected RemoveSentInviteResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

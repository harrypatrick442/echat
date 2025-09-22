using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class AddReceivedInviteResponse : TicketedMessageBase
    {
        [JsonPropertyName(AddReceivedInviteResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = AddReceivedInviteResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public AddReceivedInviteResponse(bool success, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
        protected AddReceivedInviteResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

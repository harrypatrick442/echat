using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Core.Messages.Messages;
using Flagging.DataMemberNames.Responses;

namespace Flagging.Messages.Responses
{
    [DataContract]
    public class FlagResponse : TicketedMessageBase
    {
        [JsonPropertyName(FlagResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = FlagResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public FlagResponse(bool success, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
        protected FlagResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

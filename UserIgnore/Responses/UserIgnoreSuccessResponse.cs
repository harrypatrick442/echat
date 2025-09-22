using Core.DataMemberNames;
using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Interserver.Responses;

namespace UserIgnore.Responses
{
    [DataContract]
    public class UserIgnoreSuccessResponse : TicketedMessageBase
    {
        [JsonPropertyName(UserIgnoreSuccessResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = UserIgnoreSuccessResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        public UserIgnoreSuccessResponse(
            bool success, long ticket) : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
        protected UserIgnoreSuccessResponse() : base(TicketedMessageType.Ticketed) { }
    }
}

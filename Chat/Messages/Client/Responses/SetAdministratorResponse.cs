using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class SetAdministratorResponse : TicketedMessageBase
    {
        [JsonPropertyName(SetAdministratorResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = SetAdministratorResponseDataMemberNames.FailedReason)]
        public AdministratorsFailedReason? FailedReason { get; protected set; }
        public SetAdministratorResponse(AdministratorsFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected SetAdministratorResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

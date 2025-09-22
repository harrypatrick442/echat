using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class RemoveAdministratorResponse : TicketedMessageBase
    {
        [JsonPropertyName(RemoveAdministratorResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = RemoveAdministratorResponseDataMemberNames.FailedReason)]
        public AdministratorsFailedReason? FailedReason { get; protected set; }
        public RemoveAdministratorResponse(AdministratorsFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected RemoveAdministratorResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

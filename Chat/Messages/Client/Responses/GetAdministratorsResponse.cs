using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetAdministratorsResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetAdministratorsResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = GetAdministratorsResponseDataMemberNames.FailedReason)]
        public AdministratorsFailedReason? FailedReason { get; protected set; }
        [JsonPropertyName(GetAdministratorsResponseDataMemberNames.Administrators)]
        [JsonInclude]
        [DataMember(Name = GetAdministratorsResponseDataMemberNames.Administrators)]
        public Administrator[]? Administrators{ get; protected set; }
        public GetAdministratorsResponse(Administrator[]? administrators, AdministratorsFailedReason? failedReason, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Administrators = administrators;
            FailedReason = failedReason;
            Ticket = ticket;
        }
        protected GetAdministratorsResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

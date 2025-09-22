using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Chat.Messages.Client.Messages;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetMySentInvitesResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetMySentInvitesResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = GetMySentInvitesResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(GetMySentInvitesResponseDataMemberNames.Invites)]
        [JsonInclude]
        [DataMember(Name = GetMySentInvitesResponseDataMemberNames.Invites)]
        public Invites Invites { get; protected set; }
        public GetMySentInvitesResponse(bool success, Invites invites, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Invites = invites;
            Ticket = ticket;
        }
        protected GetMySentInvitesResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

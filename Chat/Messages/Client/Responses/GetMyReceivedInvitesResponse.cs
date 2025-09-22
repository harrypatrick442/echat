using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Chat.Messages.Client.Messages;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class GetMyReceivedInvitesResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetMyReceivedInvitesResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = GetMyReceivedInvitesResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(GetMyReceivedInvitesResponseDataMemberNames.Invites)]
        [JsonInclude]
        [DataMember(Name = GetMyReceivedInvitesResponseDataMemberNames.Invites)]
        public Invites Invites { get; protected set; }
        public GetMyReceivedInvitesResponse(bool success, Invites invites, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Invites = invites;
            Ticket = ticket;
        }
        protected GetMyReceivedInvitesResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Responses;
using Core.DataMemberNames;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Responses
{
    [DataContract]
    public class RemoveReceivedInviteResponse : TicketedMessageBase
    {
        [JsonPropertyName(RemoveReceivedInviteResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = RemoveReceivedInviteResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(RemoveReceivedInviteResponseDataMemberNames.UserIdsInvitingRemoved)]
        [JsonInclude]
        [DataMember(Name = RemoveReceivedInviteResponseDataMemberNames.UserIdsInvitingRemoved)]
        public long[] UserIdsInvitingRemoved { get; protected set; }
        public RemoveReceivedInviteResponse(bool success, long[] userIdsInvitingRemoved, long ticket)
            : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            UserIdsInvitingRemoved = userIdsInvitingRemoved;
            Ticket = ticket;
        }
        protected RemoveReceivedInviteResponse()
            : base(TicketedMessageType.Ticketed) { }
    }
}

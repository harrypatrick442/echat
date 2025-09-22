using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Core.Messages.Messages;
using Users.DataMemberNames.Interserver.Responses;

namespace Users.Messages.Client
{
    [DataContract]
    public class UsernameSearchRemoveUserResponse : TicketedMessageBase
    {
        [JsonPropertyName(UsernameSearchRemoveUserResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchRemoveUserResponseDataMemberNames.Success)]
        public bool Success { get; set; }
        public UsernameSearchRemoveUserResponse(bool success, long ticket) : 
            base(TicketedMessageType.Ticketed)
        {
            Success = success;
            Ticket = ticket;
        }
        protected UsernameSearchRemoveUserResponse() :
            base(TicketedMessageType.Ticketed)
        { }

    }
}

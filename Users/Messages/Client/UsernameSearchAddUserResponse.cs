
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Core.Messages.Messages;
using Users.DataMemberNames.Interserver.Responses;

namespace Users.Messages.Client
{
    [DataContract]
    public class UsernameSearchAddUserResponse : TicketedMessageBase
    {
        [JsonPropertyName(UsernameSearchAddUserResponseDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchAddUserResponseDataMemberNames.UserId)]
        public bool Successful { get; protected set; }
        public UsernameSearchAddUserResponse(bool successful, long ticket) : 
            base(TicketedMessageType.Ticketed)
        {
            Successful = successful;
            Ticket = ticket;
        }
        protected UsernameSearchAddUserResponse() :
            base(TicketedMessageType.Ticketed)
        { }

    }
}

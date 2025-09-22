using Core.DataMemberNames;
using Core.Messages.Messages;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Responses;

namespace UserIgnore
{
    [DataContract]
    public class GetUserIgnoresResponse : TicketedMessageBase
    {
        [JsonPropertyName(GetUserIgnoresResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = GetUserIgnoresResponseDataMemberNames.Success)]
        public bool Success { get; protected set; }
        [JsonPropertyName(GetUserIgnoresResponseDataMemberNames.UserIgnores)]
        [JsonInclude]
        [DataMember(Name = GetUserIgnoresResponseDataMemberNames.UserIgnores)]
        public UserIgnores? UserIgnores { get; protected set; }
        public GetUserIgnoresResponse(
                bool success, UserIgnores? userIgnores, long ticket) : base(TicketedMessageType.Ticketed)
        {
            Success = success;
            UserIgnores = userIgnores;
            Ticket = ticket;
        }
        protected GetUserIgnoresResponse() : base(TicketedMessageType.Ticketed)
        { }
    }
}

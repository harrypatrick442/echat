
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Requests
{
    [DataContract]
    [KnownType(typeof(LogInGuestRequest))]
    public class LogInGuestRequest : TicketedMessageBase
    {
        [JsonPropertyName(LogInGuestRequestDataMemberNames.Username)]
        [JsonInclude]
        [DataMember(Name = LogInGuestRequestDataMemberNames.Username)]
        public string Username { get; protected set; }
        public LogInGuestRequest(string username) : base(global::MessageTypes.MessageTypes.AuthenticationLogInGuest)
        {
            Username = username;
        }
        protected LogInGuestRequest() : base(global::MessageTypes.MessageTypes.AuthenticationLogInGuest)
        {

        }
    }
}

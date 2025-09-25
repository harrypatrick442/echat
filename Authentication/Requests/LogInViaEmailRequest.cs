using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JSON;
using Core.Messages.Messages;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Requests
{
    [DataContract]
    [KnownType(typeof(LogInViaEmailRequest))]
    public class LogInViaEmailRequest : TicketedMessageBase
    {
        private string _Email;
        [JsonPropertyName(LogInViaEmailRequestDataMemberNames.Email)]
        [JsonInclude]
        [DataMember(Name = LogInViaEmailRequestDataMemberNames.Email)]
        public string Email { get { return _Email; } protected set { _Email = value; } }
        public LogInViaEmailRequest() : base(MessageTypes.AuthenticationLogInViaEmail)
        {

        }
    }
}

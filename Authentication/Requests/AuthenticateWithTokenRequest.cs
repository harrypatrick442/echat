using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Requests
{
    [DataContract]
    public class AuthenticateWithTokenRequest : TicketedMessageBase
    {

        public AuthenticateWithTokenRequest(string guid) : base(global::MessageTypes.MessageTypes.AuthenticateWithToken)
        {
            Token = guid;
        }
        protected AuthenticateWithTokenRequest(): base(global::MessageTypes.MessageTypes.AuthenticateWithToken)
        {

        }

        [JsonPropertyName(AuthenticateWithTokenRequestDataMemberNames.Token)]
        [JsonInclude]
        [DataMember(Name = AuthenticateWithTokenRequestDataMemberNames.Token)]
        public string Token { get; protected set; }
    }
}

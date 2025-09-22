using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Requests
{
    [DataContract]
    [KnownType(typeof(ResetPasswordByEmailRequest))]
    public class ResetPasswordByEmailRequest : TicketedMessageBase
    {
        [JsonPropertyName(ResetPasswordByEmailRequestDataMemberNames.Email)]
        [JsonInclude]
        [DataMember(Name = ResetPasswordByEmailRequestDataMemberNames.Email)]
        public string Email { get; protected set; }

        public ResetPasswordByEmailRequest(string email) : base(global::MessageTypes.MessageTypes.AuthenticationResetPasswordByEmail)
        {
            Email = email;
        }
        protected ResetPasswordByEmailRequest() : base(global::MessageTypes.MessageTypes.AuthenticationResetPasswordByEmail)
        {

        }
    }
}

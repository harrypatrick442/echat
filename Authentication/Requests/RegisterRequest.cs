using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Requests
{
    [DataContract]
    [KnownType(typeof(RegisterRequest))]
    public class RegisterRequest : TicketedMessageBase
    {
        [JsonPropertyName(RegisterRequestDataMemberNames.Email)]
        [JsonInclude]
        [DataMember(Name = RegisterRequestDataMemberNames.Email)]
        public string Email { get; protected set; }
        [JsonPropertyName(RegisterRequestDataMemberNames.Phone)]
        [JsonInclude]
        [DataMember(Name = RegisterRequestDataMemberNames.Phone)]
        public string Phone { get; protected set; }
        [JsonPropertyName(RegisterRequestDataMemberNames.Username)]
        [JsonInclude]
        [DataMember(Name = RegisterRequestDataMemberNames.Username)]
        public string Username { get; protected set; }

        [JsonPropertyName(RegisterRequestDataMemberNames.Password)]
        [JsonInclude]
        [DataMember(Name = RegisterRequestDataMemberNames.Password)]
        public string Password { get; protected set; }

        public RegisterRequest(string email, string password) : base(global::MessageTypes.MessageTypes.AuthenticationRegister)
        {
            Email = email;
            Password = password;
        }
        protected RegisterRequest() : base(global::MessageTypes.MessageTypes.AuthenticationRegister)
        {

        }
    }
}

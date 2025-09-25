
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JSON;
using Core.Messages.Messages;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Requests
{
    [DataContract]
    [KnownType(typeof(LogInRequest))]
    public class LogInRequest : TicketedMessageBase
    {
        private string _Username;
        [JsonPropertyName(LogInRequestDataMemberNames.Username)]
        [JsonInclude]
        [DataMember(Name = LogInRequestDataMemberNames.Username)]
        public string Username { get { return _Username; } protected set { _Username = value; } }
        private string _Email;
        [JsonPropertyName(LogInRequestDataMemberNames.Email)]
        [JsonInclude]
        [DataMember(Name = LogInRequestDataMemberNames.Email)]
        public string Email { get { return _Email; } protected set { _Email = value; } }
        private string _Phone;
        [JsonPropertyName(LogInRequestDataMemberNames.Phone)]
        [JsonInclude]
        [DataMember(Name = LogInRequestDataMemberNames.Phone)]
        public string Phone { get { return _Phone; } protected set { _Phone = value; } }
        private string _Password;
        [JsonPropertyName(LogInRequestDataMemberNames.Password)]
        [JsonInclude]
        [DataMember(Name = LogInRequestDataMemberNames.Password)]
        public string Password { get { return _Password; } protected set { _Password = value; } }
        private int? _UserId;
        [JsonPropertyName(LogInRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = LogInRequestDataMemberNames.UserId)]
        public int? UserId { get { return _UserId; } protected set { _UserId = value; } }

        public LogInRequest(string email, string password) : base(MessageTypes.AuthenticationLogIn)
        {
            _Email = email;
            _Password = password;
        }
        protected LogInRequest() : base(MessageTypes.AuthenticationLogIn)
        {

        }
    }
}

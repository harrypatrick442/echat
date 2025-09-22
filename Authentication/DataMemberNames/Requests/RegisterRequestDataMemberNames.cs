using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.AuthenticationRegister)]
    public class RegisterRequestDataMemberNames
    {
        public const string Email = "e";
        public const string Phone = "n";
        public const string Password = "p";
        public const string Username = "u";
    }
}
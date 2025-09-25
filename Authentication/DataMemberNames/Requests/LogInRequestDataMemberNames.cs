using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(MessageTypes.AuthenticationLogIn)]
    public class LogInRequestDataMemberNames
    {
        public const string Username = "un";
        public const string Email = "e";
        public const string Phone = "n";
        public const string Password = "p";
        public const string UserId = "u";
    }
}
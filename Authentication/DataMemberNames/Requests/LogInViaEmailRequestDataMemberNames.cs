using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(MessageTypes.AuthenticationLogIn)]
    public class LogInViaEmailRequestDataMemberNames
    {
        public const string Email = "e";
    }
}
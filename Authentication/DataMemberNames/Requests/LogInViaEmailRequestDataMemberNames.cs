using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.AuthenticationLogIn)]
    public class LogInViaEmailRequestDataMemberNames
    {
        public const string Email = "e";
    }
}
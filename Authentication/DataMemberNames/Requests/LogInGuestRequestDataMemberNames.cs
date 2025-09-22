using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.AuthenticationLogInGuest)]
    public class LogInGuestRequestDataMemberNames
    {
        public const string Username = "u";
    }
}
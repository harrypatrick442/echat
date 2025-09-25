using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(MessageTypes.AuthenticationLogInGuest)]
    public class LogInGuestRequestDataMemberNames
    {
        public const string Username = "u";
    }
}
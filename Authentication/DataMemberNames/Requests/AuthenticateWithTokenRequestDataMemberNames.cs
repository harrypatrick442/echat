using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{

    [MessageType(MessageTypes.AuthenticateWithToken)]
    public class AuthenticateWithTokenRequestDataMemberNames
    {
        public const string Token = "u";
    }
}
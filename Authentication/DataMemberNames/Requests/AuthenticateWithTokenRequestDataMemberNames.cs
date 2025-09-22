using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{

    [MessageType(global::MessageTypes.MessageTypes.AuthenticateWithToken)]
    public class AuthenticateWithTokenRequestDataMemberNames
    {
        public const string Token = "u";
    }
}
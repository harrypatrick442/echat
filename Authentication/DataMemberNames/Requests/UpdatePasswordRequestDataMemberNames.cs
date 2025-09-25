using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(MessageTypes.AuthenticationUpdatePassword)]
    public class UpdatePasswordRequestDataMemberNames
    {
        public const string Secret = "secret";
        public const string Password = "password";
    }
}
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;

namespace Authentication.Messages
{
    [DataContract]
    public class AuthenticationFailedMessage : TypedMessageBase
    {
        public AuthenticationFailedMessage()
        {
            _Type = "authenticationFailed";
        }
    }
}

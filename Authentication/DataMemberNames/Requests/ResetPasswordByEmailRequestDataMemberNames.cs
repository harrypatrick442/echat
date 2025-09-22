using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.AuthenticationResetPasswordByEmail)]
    public class ResetPasswordByEmailRequestDataMemberNames
    {
        public const string Email = "e";
    }
}
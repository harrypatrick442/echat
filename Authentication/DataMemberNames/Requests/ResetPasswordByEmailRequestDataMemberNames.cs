using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Requests
{
    [MessageType(MessageTypes.AuthenticationResetPasswordByEmail)]
    public class ResetPasswordByEmailRequestDataMemberNames
    {
        public const string Email = "e";
    }
}
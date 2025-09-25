using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication
{
    public static class MessageTypes
    {
        public const string
            TokenAuthenticationResult = "ta",
            AuthenticateWithToken = "at",
             AuthenticationLogIn = "ali",
             AuthenticationLogInGuest = "alig",
             AuthenticationLogOut = "alo",
             AuthenticationRegister = "ar",
             AuthenticationLogInViaEmail = "alive",
             AuthenticationDoNotHavePermission = "adnhp",
             AuthenticationResetPasswordByEmail = "arpbe",
             AuthenticationUpdatePassword = "aup";
    }
}

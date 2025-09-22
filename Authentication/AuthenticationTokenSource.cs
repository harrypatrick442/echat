
using Authentication.DAL;

namespace Authentication
{
    public class AuthenticationTokenSource
    {
        public static string Next(IDalAuthentication dalAuthentication) {
            string guid;
            do
            {
                guid = System.Guid.NewGuid().ToString("N");
            }
            while (dalAuthentication.GetAuthenticationTokenByGuid(guid) != null);
            return guid;
        }
    }
}

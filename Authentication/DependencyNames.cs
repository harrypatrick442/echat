using Core.Limiters;

namespace Authentication
{
    public static class DependencyNames
    {
        public const string 
            UserIdToAuthenticationInfoDatabaseFilePath = "UserIdToAuthenticationInfoDatabaseFilePath",
            EmailToAuthenticationInfoDatabaseFilePath = "EmailToAuthenticationInfoDatabaseFilePath",
            UsernameToAuthenticationInfoDatabaseFilePath = "UsernameToAuthenticationInfoDatabaseFilePath",
            PhoneToAuthenticationInfoDatabaseFilePath = "PhoneToAuthenticationInfoDatabaseFilePath",
            UserIdToAuthenticationTokenDatabaseFilePath = "UserIdToAuthenticationTokenDatabaseFilePath",
            GuidToAuthenticationTokenDatabaseFilePath = "GuidToAuthenticationTokenDatabaseFilePath";
    }
}

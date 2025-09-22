using Core.DTOs;

namespace Authentication.DAL
{
    public interface IDalAuthentication
    {

        public bool EmailAlreadyRegistered(string email);
        public AuthenticationInfo GetAuthenticationInfoByEmail(string email);
        public AuthenticationInfo GetAuthenticationInfoByPhone(string phone);
        public AuthenticationInfo GetAuthenticationInfoByUsername(string username);
        public AuthenticationInfo GetAuthenticationInfoByUserId(long userId);
        public AuthenticationToken GetAuthenticationTokenByUserIdAndDevice(long userId, string deviceIdentifier);
        public void CreateAuthenticationInfo(AuthenticationInfo newAuthenticationInfo);
        public void ModifyAuthenticationInfo(long userId, Func<AuthenticationInfo, AuthenticationInfo> callback);
        public AuthenticationToken GetAuthenticationTokenByGuid(string guid);
        public void AddOrSetAuthenticationTokenForUserIdAndDevice(AuthenticationToken newAuthenticationToken);
    }
}
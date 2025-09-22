using Core.DTOs;
namespace Authentication.DAL
{
    public class SingleUserHardCodedDalAuthentication : IDalAuthentication
    {
        private AuthenticationInfo _AuthenticationInfoNormalized;
        private bool _AnyCredentialsReturnAuthenticationInfo;
        public SingleUserHardCodedDalAuthentication(AuthenticationInfo authenticationInfo, bool anyCredentialsReturnAuthenticationInfo) {
            _AuthenticationInfoNormalized = new AuthenticationInfo(
                authenticationInfo.UserId,
                authenticationInfo.Hash, 
                StringsHelper.NormalizeEmail(authenticationInfo.Email),
                //null,
                StringsHelper.NormalizePhone(authenticationInfo.Phone)
            );
            _AnyCredentialsReturnAuthenticationInfo = anyCredentialsReturnAuthenticationInfo;
        }
        public void AddOrSetAuthenticationTokenForUserIdAndDevice(AuthenticationToken newAuthenticationToken)
        {
            
        }

        public void CreateAuthenticationInfo(AuthenticationInfo newAuthenticationInfo)
        {
            
        }

        public AuthenticationInfo GetAuthenticationInfoByEmail(string email)
        {
            if (_AnyCredentialsReturnAuthenticationInfo)
                return _AuthenticationInfoNormalized;
            if (email == null) return null;
            if (_AuthenticationInfoNormalized.Email == StringsHelper.NormalizeEmail(email))
                return _AuthenticationInfoNormalized;
            return null;
        }

        public AuthenticationInfo GetAuthenticationInfoByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public AuthenticationInfo GetAuthenticationInfoByPhone(string phone)
        {
            if (_AnyCredentialsReturnAuthenticationInfo)
                return _AuthenticationInfoNormalized;
            if (phone == null) return null;
            if (_AuthenticationInfoNormalized.Email == StringsHelper.NormalizePhone(phone))
                return _AuthenticationInfoNormalized;
            return null;
        }

        public AuthenticationInfo GetAuthenticationInfoByUserId(long userId)
        {
            if (_AnyCredentialsReturnAuthenticationInfo||_AuthenticationInfoNormalized.UserId == userId)
                return _AuthenticationInfoNormalized;
            return null;
        }

        public AuthenticationToken GetAuthenticationTokenByGuid(string guid)
        {
            return null;
        }

        public AuthenticationToken GetAuthenticationTokenByUserIdAndDevice(long userId, string deviceIdentifier)
        {
            return null;
        }

        public void ModifyAuthenticationInfo(long userId, Func<AuthenticationInfo, AuthenticationInfo> callback)
        {
            throw new NotImplementedException();
        }

        public bool EmailAlreadyRegistered(string email)
        {
            throw new NotImplementedException();
        }
    }
}
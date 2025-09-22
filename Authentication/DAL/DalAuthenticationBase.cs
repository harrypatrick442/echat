using JSON;
using Core.DTOs;
using Core.Enums;
using Core.Exceptions;
using KeyValuePairDatabases;
using DevOne.Security.Cryptography.BCrypt;
using System.Security.Policy;

namespace Authentication.DAL
{
    public class DalAuthenticationBase
    {
        protected IKeyValuePairDatabase<long, AuthenticationInfo> _UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase
           ;
        protected IKeyValuePairDatabase<string, AuthenticationInfo> _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase
           ;
        protected IKeyValuePairDatabase<string, AuthenticationInfo> _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase
            ;
        protected IKeyValuePairDatabase<string, AuthenticationInfo> _UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase;
        
        protected IKeyValuePairDatabase<long, AuthenticationTokens> _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase
            ;
        protected IKeyValuePairDatabase<string, AuthenticationToken> _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase
        
           ;//TODO on disk only

        private Json _NativeJsonParser = new Json();
        protected DalAuthenticationBase()
        {

        }
        public AuthenticationInfo GetAuthenticationInfoByEmail(string email)
        {
            string identifier = StringsHelper.NormalizeEmail(email);
            return _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase.Get(identifier);
        }
        public void UpdatePassword(long userId, string password) {
            ModifyAuthenticationInfo(userId, (oldAuthenticationInfo) => {
                string newHash = BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt());
                return new AuthenticationInfo(userId, newHash, oldAuthenticationInfo.Email, oldAuthenticationInfo.Phone);
            });
        }
        public bool EmailAlreadyRegistered(string email)
        {
            string identifier = StringsHelper.NormalizeEmail(email);
            return _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase.HasNotCountingNull(identifier);
        }
        public AuthenticationInfo GetAuthenticationInfoByPhone(string phone)
        {
            string identifier = StringsHelper.NormalizePhone(phone);
            return _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase.Get(identifier);
        }
        public AuthenticationInfo GetAuthenticationInfoByUsername(string username)
        {
            string identifier = username;
            return _UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase.Get(identifier);
        }
        public AuthenticationInfo GetAuthenticationInfoByUserId(long userId)
        {
            return _UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase.Get(userId);
        }
        public AuthenticationToken GetAuthenticationTokenByUserIdAndDevice(long userId, string deviceIdentifier)
        {
            AuthenticationTokens authenticationTokens = _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase.Get(userId);
            if (authenticationTokens == null || authenticationTokens.Entries == null) return null;
            return authenticationTokens.Entries.Where(authenticationToken => authenticationToken.DeviceIdentifier == deviceIdentifier).FirstOrDefault();
        }
        public void CreateAuthenticationInfo(AuthenticationInfo newAuthenticationInfo)
        {
            if (newAuthenticationInfo.UserId < 1)
                throw new ArgumentException(nameof(newAuthenticationInfo.UserId));
            string emailNormalized = StringsHelper.NormalizeEmail(newAuthenticationInfo.Email);
            string phoneNormalized = StringsHelper.NormalizePhone(newAuthenticationInfo.Phone);
            //string username = newAuthenticationInfo.Username;
            bool hasEmail = !string.IsNullOrWhiteSpace(emailNormalized);
            bool hasPhone = !string.IsNullOrWhiteSpace(phoneNormalized);
            //bool hasUsername = !string.IsNullOrWhiteSpace(username);
            _UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase.ModifyWithinLock(newAuthenticationInfo.UserId,
                (existingAuthenticationInfoShouldntExist) =>
                {
                    if (existingAuthenticationInfoShouldntExist != null)
                        throw new UserIdInvalidException(UserIdInvalidReason.AlreadyInUse);

                    List<Action> sets = new List<Action>(3);
                    int setIndex = 0;
                    Action nextSet = () =>
                    {
                        if (setIndex >= sets.Count) return;
                        sets[setIndex++]();
                    };
                    if (hasEmail)
                    {
                        bool emailAlreadyRegistered = _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase.HasNotCountingNull(emailNormalized);
                        if (emailAlreadyRegistered)
                        {
                            throw new EmailInvalidException(EmailInvalidReason.AlreadyInUse);
                        }
                        sets.Add(() =>
                        {
                            _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase.ModifyWithinLock(emailNormalized, (shouldBeNull) =>
                            {
                                //Extremely unlikely to be able to reach this but its just incase
                                if (shouldBeNull != null)
                                    throw new EmailInvalidException(EmailInvalidReason.AlreadyInUse);
                                nextSet();
                                return newAuthenticationInfo;
                            });
                        });
                    }
                    if (hasPhone)
                    {
                        bool phoneAlreadyRegistered = _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase.HasNotCountingNull(phoneNormalized);
                        if (phoneAlreadyRegistered)
                        {
                            throw new PhoneInvalidException(PhoneInvalidReason.AlreadyInUse);
                        }
                        sets.Add(() =>
                        {
                            _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase.ModifyWithinLock(phoneNormalized, (shouldBeNull) =>
                            {
                                if (shouldBeNull != null)
                                    throw new PhoneInvalidException(PhoneInvalidReason.AlreadyInUse);
                                nextSet();
                                return newAuthenticationInfo;
                            });
                        });
                    }
                    /*
                    if (hasUsername)
                    {
                        bool usernameAlreadyRegistered = _UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase.HasNotCountingNull(username);
                        if (usernameAlreadyRegistered)
                        {
                            throw new UsernameInvalidException(UsernameInvalidReason.AlreadyInUse);
                        }
                        sets.Add(() =>
                        {
                            _UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase.ModifyWithinLock(username, (shouldBeNull) =>
                            {
                                if (shouldBeNull != null)
                                    throw new UsernameInvalidException(UsernameInvalidReason.AlreadyInUse);
                                nextSet();
                                return newAuthenticationInfo;
                            });
                        });
                    }*/
                    nextSet();
                    return newAuthenticationInfo;
                });
        }
        public void ModifyAuthenticationInfo(long userId, Func<AuthenticationInfo, AuthenticationInfo> callback)
        {
            _UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase.ModifyWithinLock(userId,
                (currentAuthenticationInfo) =>
                {
                    string oldEmailNormalized = null, oldPhoneNormalized = null;
                    //WARNING They may modify currentAuthenticationInfo in callback so
                    //get value out of it first and do not touch it after calling callback!
                    if (currentAuthenticationInfo != null)
                    {
                        oldEmailNormalized = StringsHelper.NormalizeEmail(currentAuthenticationInfo.Email);
                        oldPhoneNormalized = StringsHelper.NormalizePhone(currentAuthenticationInfo.Phone);
                    }
                    AuthenticationInfo newAuthenticationInfo = callback(currentAuthenticationInfo);
                    if (newAuthenticationInfo == null) throw new Exception($"{nameof(newAuthenticationInfo)} should never be null. Use the delete method if you wish to delete it!");
                    string newEmailNormalized = StringsHelper.NormalizeEmail(newAuthenticationInfo.Email);
                    string newPhoneNormalized = StringsHelper.NormalizePhone(newAuthenticationInfo.Phone);

                    if (!string.IsNullOrEmpty(newEmailNormalized))
                    {
                        _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase.Set(newEmailNormalized, newAuthenticationInfo);
                    }
                    if (oldEmailNormalized != newEmailNormalized)
                    {
                        if ((!string.IsNullOrEmpty(oldEmailNormalized)))
                        {
                            _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase.Delete(oldEmailNormalized);
                        }
                    }
                    if (!string.IsNullOrEmpty(newPhoneNormalized))
                    {
                        _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase.Set(newPhoneNormalized, newAuthenticationInfo);
                    }
                    if (oldPhoneNormalized != newPhoneNormalized)
                    {
                        if ((!string.IsNullOrEmpty(oldPhoneNormalized)))
                        {
                            _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase.Delete(oldPhoneNormalized);
                        }
                    }
                    return newAuthenticationInfo;
                });
        }
        public AuthenticationToken GetAuthenticationTokenByGuid(string guid)
        {
            return _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase.Get(guid);
        }
        /*
        public void UpdateAuthenticationToken(int userId, string oldGuid, string newGuid)
        {
            if (userId <= 0)
                throw new ArgumentException($"invalid {nameof(userId)}");
            string userIdString = userId.ToString();

            _SetAuthenticationTokensLock.LockForWrite(userIdString, () =>
            {
                AuthenticationToken existingAuthenticationToken = _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase.Read(oldGuid);
                if (existingAuthenticationToken == null)
                    throw new ArgumentException($"{nameof(oldGuid)} was invalid");
                _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase.Delete(oldGuid);
                AuthenticationTokens authenticationTokens =
                    _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase.Read(userIdString);
                existingAuthenticationToken.Guid = newGuid;
                if (authenticationTokens != null)
                {
                    foreach (AuthenticationToken oldEntry in authenticationTokens.Entries)
                    {
                        if (oldEntry.Guid == oldGuid)
                        {
                            oldEntry.Guid = newGuid;
                        }
                    }
                }
                else authenticationTokens = new AuthenticationTokens(new AuthenticationToken[] { existingAuthenticationToken });
                _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase.Write(userIdString, authenticationTokens);
                _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase.Write(newGuid, existingAuthenticationToken);

            });
        }*/
        public void AddOrSetAuthenticationTokenForUserIdAndDevice(AuthenticationToken newAuthenticationToken)
        {
            long userId = newAuthenticationToken.UserId;
            if (userId <= 0)
                throw new ArgumentException($"{nameof(AuthenticationToken)} had an invalid {nameof(newAuthenticationToken.UserId)}");
            string userIdString = userId.ToString();
            _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase.ModifyWithinLock(userId, (existingAuthenticationTokens) => {
                if (existingAuthenticationTokens == null || existingAuthenticationTokens.Entries == null)
                {

                    _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase.Set(newAuthenticationToken.Guid, newAuthenticationToken);
                    return new AuthenticationTokens(new AuthenticationToken[] { newAuthenticationToken });
                }
                List<AuthenticationToken> authenticationTokensBeingKept = new List<AuthenticationToken>() { newAuthenticationToken };
                foreach (AuthenticationToken existingAuthenticationToken
                        in existingAuthenticationTokens.Entries)
                {
                    bool hasSameDeviceIdentifier = existingAuthenticationToken.DeviceIdentifier
                            == newAuthenticationToken.DeviceIdentifier;

                    if (hasSameDeviceIdentifier)
                    {
                        _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase.Delete(existingAuthenticationToken.Guid);
                    }
                    else
                    {
                        authenticationTokensBeingKept.Add(existingAuthenticationToken);
                    }
                }
                _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase.Set(newAuthenticationToken.Guid, newAuthenticationToken);
                return new AuthenticationTokens(authenticationTokensBeingKept.ToArray());
            });
        }
    }
}
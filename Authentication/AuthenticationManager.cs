
using Authentication.DAL;
using Authentication.Enums;
using Authentication.Requests;
using Core.DTOs;
using Core.Enums;
using Core.Exceptions;
using Core.Timing;
using DevOne.Security.Cryptography.BCrypt;
using Emailing;
using GlobalConstants;
using Logging;
using Sessions;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Authentication
{
    public class AuthenticationManager
    {
        private static AuthenticationManager _Instance;
        public static AuthenticationManager Instance { get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(AuthenticationManager));
                return _Instance; } }
        public static AuthenticationManager Initialize(
            int authenticationNodeId,
            DalAuthentication dalAuthentication,
            IAuthenticationRelatedUserInfoSource authenticationRelatedUserInfoSource,
            CredentialsSetup credentialsSetup,
            IAuthenticationEmailer authenticationEmailing) {
            if (_Instance != null) throw new NotInitializedException(nameof(AuthenticationManager));
            _Instance = new AuthenticationManager(
                authenticationNodeId,
                dalAuthentication,
                authenticationRelatedUserInfoSource,
                credentialsSetup,
                authenticationEmailing
            );
            return _Instance;
        }
        private int _AuthenticationNodeId;
        private DalAuthentication _DalAuthentication;
        private IAuthenticationRelatedUserInfoSource _AuthenticationRelatedUserInfoSource;
        public CredentialsSetup CredentialsSetup { get; }
        private IAuthenticationEmailer _AuthenticationEmailing;


        private AuthenticationManager(
            int authenticationNodeId,
            DalAuthentication dalAuthentication,
            IAuthenticationRelatedUserInfoSource authenticationRelatedUserInfoSource,
            CredentialsSetup credentialsSetup,
            IAuthenticationEmailer authenticationEmailing)
        {
            _AuthenticationNodeId = authenticationNodeId;
            _DalAuthentication = dalAuthentication;
            _AuthenticationRelatedUserInfoSource = authenticationRelatedUserInfoSource;
            CredentialsSetup = credentialsSetup;
            _AuthenticationEmailing = authenticationEmailing;

        }
        public long Register(
            DelegateCreateNewUser createNewUser,
            string email,
            string username, 
            string phone, 
            string password,
            IPAddress clientIPAddress,
            string deviceIdentifier) {
            CheckNotTooFrequently(clientIPAddress, out Action successful);
            bool hasUsername = CheckUsernameValid(username);
            bool hasEmail = CheckEmailValid(email);
            bool hasPhone = CheckPhoneValid(phone);
            CheckPasswordValid(password);

            if ((CredentialsSetup.EmailRequiredToRegister&&!hasEmail) 
                || (CredentialsSetup.PhoneRequiredToRegister&&!hasPhone) 
                || (CredentialsSetup.UsernameRequiredToRegister&&!hasUsername))
                throw new BadCredentialsException();
            string hash = BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt());
            long userId = createNewUser(guest:false, username);
            AuthenticationInfo authenticationInfo =new AuthenticationInfo(
                userId, 
                hash, 
                email,
                //_CredentialsSetup.UsernamesUnique?username: null, 
                phone);
            _DalAuthentication.CreateAuthenticationInfo(authenticationInfo);
            /*Does lots of checks such as email already in use*/
            return userId;
        }
        public long LogInGuest(
            DelegateCreateNewUser createNewUser,
            IPAddress clientIPAddress,
            string username)
        {
            if (!CredentialsSetup.GuestEnabled)
                throw new GuestNotEnabledException();
            CheckUsernameValid(username);
            CheckNotTooFrequently(clientIPAddress, out Action successful);
            long userId = createNewUser(guest: true, username);
            return userId;
        }
        public SessionInfo LogIn(LogInRequest authenticateRequest,
            IPAddress clientIPAddress, string deviceIdentifier,
            ISessionIdSource sessionIdSource)
        {
            CheckNotTooFrequently(clientIPAddress, out Action successful);
            SessionInfo sessionInfo = LogIn_ByPassword(authenticateRequest,
                deviceIdentifier, sessionIdSource);
            try
            {
                successful();
            }
            catch(Exception ex) {
                sessionInfo.Dispose();
                throw new OperationFailedException(nameof(successful), ex);
            }
            return sessionInfo;
        }
        public bool ResetPasswordByEmail(string email,
                    string operatingSystem,
                    string browserName
            ) {
            try
            {
                AuthenticationInfo authenticationInfo = _DalAuthentication.GetAuthenticationInfoByEmail(email);
                if (authenticationInfo == null) return true;
                string secret = PasswordResetManager.Instance.Prepare(authenticationInfo.UserId);
#if DEBUG
    string origin = "https://localhost:7161";
#else
                    string domainName = GlobalConstants.Nodes.FirstUniqueDomainForNode(
                        _AuthenticationNodeId);
                    string origin = $"https://{domainName}";
#endif
                string actionUrl =  $"{origin}/{RoutesWithoutSlash.RESET_PASSWORD_CLICKED_LINK}/{secret}";
                string username = _AuthenticationRelatedUserInfoSource
                    .GetInfoForEmailing(authenticationInfo.UserId);
                _AuthenticationEmailing.SendPasswordResetEmail(
                    authenticationInfo.Email,
                    username,
                    operatingSystem,
                    browserName,
                    actionUrl
                    );
                return true;
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                return false;
            }
        }
        /*
        public static void LogInViaEmail(LogInViaEmailRequest authenticateRequest,
            IPAddress clientIPAddress, string deviceIdentifier)
        {

            _CheckNotTooFrequently(clientIPAddress, out Action successful);
            _SendAuthenticationEmail(authenticateRequest.Email);
            successful();
        }*/
        public SessionInfo TryToAuthenticateWithToken(string token,
            string deviceIdentifier, ISessionIdSource sessionIdSource)
        {
            AuthenticationToken authenticationToken = _DalAuthentication.GetAuthenticationTokenByGuid(
                token);
            if (authenticationToken != null)
            {
                return Sessions.Sessions.New(authenticationToken.UserId, token, deviceIdentifier, sessionIdSource);
            }
            throw new BadCredentialsException();
        }
        private SessionInfo LogIn_ByPassword(LogInRequest authenticateRequest,
            string deviceIdentifier, ISessionIdSource sessionIdSource)
        {
            AuthenticationInfo authenticationInfo = GetAuthenticationInfoByWhatIsProvided(authenticateRequest);
            if (authenticationInfo == null)
                throw new BadCredentialsException();
            long userId = authenticationInfo.UserId;
            if (BCryptHelper.CheckPassword(authenticateRequest.Password,
                authenticationInfo.Hash))
            {
                AuthenticationToken authenticationToken = _DalAuthentication.GetAuthenticationTokenByUserIdAndDevice(userId, deviceIdentifier);
                if (authenticationToken == null)
                {
                    authenticationToken = new AuthenticationToken(
                        AuthenticationTokenSource.Next(_DalAuthentication), 
                        userId, deviceIdentifier, expiresAt: -1, TimeHelper.MillisecondsNow);
                    _DalAuthentication.AddOrSetAuthenticationTokenForUserIdAndDevice(authenticationToken);
                }
                return Sessions.Sessions.New(authenticationInfo.UserId, authenticationToken.Guid, deviceIdentifier, sessionIdSource);
            }
            throw new BadCredentialsException();
        }
        private AuthenticationInfo GetAuthenticationInfoByWhatIsProvided(
            LogInRequest authenticateRequest)
        {

            if (authenticateRequest.UserId != null)
            {
                return _DalAuthentication.GetAuthenticationInfoByUserId(
                    (int)authenticateRequest.UserId);
            }
            if (!string.IsNullOrEmpty(authenticateRequest.Email))
            {
                return _DalAuthentication.GetAuthenticationInfoByEmail(
                    authenticateRequest.Email);
            }
            /*if (!string.IsNullOrEmpty(authenticateRequest.Username))
            {
                return dalAuthentication.GetAuthenticationInfoByUsername(
                    authenticateRequest.Username);
            }*/
            if (!string.IsNullOrEmpty(authenticateRequest.Phone))
            {
                return _DalAuthentication.GetAuthenticationInfoByPhone(
                    authenticateRequest.Phone);
            }
            return null;
        }
        private void SendAuthenticationEmail(string emailAddress)
        {
            AuthenticationInfo authenticationInfo =
                _DalAuthentication.GetAuthenticationInfoByEmail(emailAddress);
            string code = AuthenticationCodeManager.Instance.CreateCode(authenticationInfo.UserId);
            string messageBody = $"Your Snippets authentication code is {code}";
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(
                from: "snippets-auth@gmail.com", to: emailAddress, subject: $"sign-in code: {code}", body: messageBody);
            using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("localhost"))
            {
                smtp.Send(message);//Handles all messages in the protocol
            }
        }
        public SessionInfo CreateToken(long userId, string deviceIdentifier,
            ISessionIdSource sessionIdSource)
        {
            AuthenticationToken authenticationToken = new AuthenticationToken(AuthenticationTokenSource.Next(_DalAuthentication), userId,
                deviceIdentifier,
                expiresAt: -1, createdAt: TimeHelper.MillisecondsNow);
            _DalAuthentication.AddOrSetAuthenticationTokenForUserIdAndDevice(authenticationToken);
            return Sessions.Sessions.New(userId, authenticationToken.Guid, deviceIdentifier, sessionIdSource);
        }
        private void CheckPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password)||password.Length < CredentialsSetup.PasswordMinLength)
                throw new PasswordInvalidException(PasswordInvalidReason.TooShort, CredentialsSetup.PasswordMinLength, CredentialsSetup.PasswordMaxLength);
            if (password.Length > CredentialsSetup.PasswordMaxLength)
                throw new PasswordInvalidException(PasswordInvalidReason.TooLong, CredentialsSetup.PasswordMinLength, CredentialsSetup.PasswordMaxLength);
        }
        private void CheckNotTooFrequently(
            IPAddress clientIPAddress, 
            out Action successful)
        {
            long millisecondsNow = TimeHelper.MillisecondsNow;
            if (AuthenticationAttemptByIPFrequencyManager.Instance.TooFrequentlyTrying(clientIPAddress,
                out int delaySecondsRequired, millisecondsNow))
                throw new MustWaitToRetryException("Too many authentication requirests", delaySecondsRequired);
            AuthenticationAttemptByIPFrequencyManager.Instance.AddTry(clientIPAddress, millisecondsNow);
            successful = () => {
                AuthenticationAttemptByIPFrequencyManager.Instance.Remove(clientIPAddress);
            };
        }
        private bool CheckUsernameValid(string username)
        {
            bool hasUsername = !string.IsNullOrWhiteSpace(username);
            if (!hasUsername) return false;
            if (string.IsNullOrEmpty(username) ||
                username.Length < CredentialsSetup.UsernameMinLength)
                throw new UsernameInvalidException(UsernameInvalidReason.TooShort,
                    CredentialsSetup.UsernameMinLength, CredentialsSetup.UsernameMaxLength);
            if (username.Length > CredentialsSetup.UsernameMaxLength)
                throw new UsernameInvalidException(UsernameInvalidReason.TooLong,
                    CredentialsSetup.UsernameMinLength, CredentialsSetup.UsernameMaxLength);
            if (CredentialsSetup.UsernamesUnique&&DalAuthentication.Instance.GetAuthenticationInfoByUsername(username) != null) {
                throw new UsernameInvalidException(UsernameInvalidReason.AlreadyInUse);
            }
            return true;
        }
        private bool CheckEmailValid(string email)
        {
            bool hasEmail = !string.IsNullOrWhiteSpace(email);
            if (!hasEmail) return false;
            try
            {
                new MailAddress(email);

            }
            catch (FormatException)
            {
                throw new EmailInvalidException(EmailInvalidReason.Invalid);
            }
            return true;

        }
        private bool CheckPhoneValid(string phone)
        {
            bool hasPhone = !string.IsNullOrWhiteSpace(phone);
            if (!hasPhone) return false;
            if (new Regex(@"^(\+[0-9]{9})$").Match(phone).Success)
                throw new EmailInvalidException(EmailInvalidReason.Invalid);
            return true;
        }
    }
}

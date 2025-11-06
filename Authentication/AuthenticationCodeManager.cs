
using Core.Exceptions;
using Core.Timing;
using Core.Maths;
using Initialization.Exceptions;

namespace Authentication
{
    public class AuthenticationCodeManager
    {
        private static AuthenticationCodeManager _Instance;
        private AuthenticationAttemptFrequencyManager _AuthenticationAttemptFrequencyManager = new AuthenticationAttemptFrequencyManager();
        private const int N_NUMERIC_DIGITS = 6;
        private const int CODE_TIMEOUT_SECONDS = 120;
        private const int CODE_TIMEOUT_MILLISECONDS = CODE_TIMEOUT_SECONDS * 1000;
        private static readonly int MAX_N_CODES_AT_ONCE = (int)(Math.Pow(10, N_NUMERIC_DIGITS) / 10);
        public static void Initialize()
        {
            if (_Instance == null)
                _Instance = new AuthenticationCodeManager();
        }
        public static AuthenticationCodeManager Instance { get {
                if (_Instance != null)
                    throw new NotInitializedException(typeof(AuthenticationCodeManager));
                return _Instance; } }
        private Dictionary<string, AuthenticationCodeAttempt> _MapCodeToAuthenticationCodeAttempt = new Dictionary<string, AuthenticationCodeAttempt>();
        protected AuthenticationCodeManager() {

        }
        public bool CheckCode(string code, int userId) {
            long millisecondsUTC = TimeHelper.MillisecondsNow;
            lock (_MapCodeToAuthenticationCodeAttempt) {
                if (!_MapCodeToAuthenticationCodeAttempt.ContainsKey(code))
                {
                    _AuthenticationAttemptFrequencyManager.AddTry(userId, millisecondsUTC);
                    return false;
                }
                AuthenticationCodeAttempt authenticationCodeAttempt = _MapCodeToAuthenticationCodeAttempt[code];
                if (millisecondsUTC >= authenticationCodeAttempt.TimesOutAtMillisecondsUTC)
                {
                    _MapCodeToAuthenticationCodeAttempt.Remove(authenticationCodeAttempt.Code);
                    _AuthenticationAttemptFrequencyManager.AddTry(userId, millisecondsUTC);
                    return false;
                }
                if (authenticationCodeAttempt.UserId != userId)
                {
                    return false;
                }
                _MapCodeToAuthenticationCodeAttempt.Remove(authenticationCodeAttempt.Code);
                _AuthenticationAttemptFrequencyManager.Remove(userId);
                return true;
            }
        }
        public string CreateCode(long userId)
        {
            lock (_MapCodeToAuthenticationCodeAttempt)
            {
                long millisecondsNowUTC = TimeHelper.MillisecondsNow;
                if (_MapCodeToAuthenticationCodeAttempt.Count > MAX_N_CODES_AT_ONCE)
                    throw new BusyException("Too many codes in use");
                if (_AuthenticationAttemptFrequencyManager.TooFrequentlyTrying(userId, out int delaySecondsRequired, millisecondsNowUTC))
                    throw new MustWaitToRetryException("You must wait before you can try again",
                        delaySecondsRequired);
                string code = null;
                do
                {
                    code = GetRandomCode();
                } while (_MapCodeToAuthenticationCodeAttempt.ContainsKey(code));
                _MapCodeToAuthenticationCodeAttempt[code]= new AuthenticationCodeAttempt(code, userId, millisecondsNowUTC+CODE_TIMEOUT_MILLISECONDS);
                return code;
            }
        }
        private void CleanupExpiredCodes() { 
            
        }
        private string GetRandomCode() {
            string str = "";
            for (int i = 0; i < N_NUMERIC_DIGITS; i++)
            {
                str += RandomHelper.NextInt(0, 10).ToString();
            }
            return str;
        }
    }
}

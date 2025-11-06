using Core.Exceptions;
using Initialization.Exceptions;
using System.Net;

namespace Authentication
{
    public class AuthenticationAttemptByIPFrequencyManager : AttemptFrequencyManager<LastTriedToAuthenticateUsingIP, IPAddress>
    {
        private static AuthenticationAttemptByIPFrequencyManager _Instance;
        public static void Initialize() {
            if(_Instance==null)
            _Instance = new AuthenticationAttemptByIPFrequencyManager();
        }
        public static AuthenticationAttemptByIPFrequencyManager Instance { get { if (_Instance == null) throw new NotInitializedException(typeof(AuthenticationAttemptByIPFrequencyManager));
                return _Instance; } }
        protected AuthenticationAttemptByIPFrequencyManager()
        {

        }
        public override bool TooFrequentlyTrying(IPAddress ipAddress, out int delaySecondsRequired, long millisecondsNowUTC)
        {
            return base.TooFrequentlyTrying(ipAddress, out delaySecondsRequired, millisecondsNowUTC);
        }
        public override void AddTry(IPAddress ipAddress, long millisecondsNowUTC)
        {
            base.AddTry(ipAddress, millisecondsNowUTC);
        }
        public override void Remove(IPAddress ipAddress)
        {
            base.Remove(ipAddress);
        }
    }
}

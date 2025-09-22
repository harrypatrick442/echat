
namespace Authentication
{
    public class AuthenticationCodeAttempt
    {
        private long _TimesOutAtMillisecondsUTC;
        public long TimesOutAtMillisecondsUTC { get { return _TimesOutAtMillisecondsUTC; } }
        private string _Code;
        public string Code { get { return _Code; } }
        private long _UserId;
        public long UserId { get { return _UserId; } }
        public AuthenticationCodeAttempt(string code, long userId, long timesOutAtMillisecondsUTC) {
            _Code = code;
            _UserId = userId;
            _TimesOutAtMillisecondsUTC = timesOutAtMillisecondsUTC;
        }
    }
}

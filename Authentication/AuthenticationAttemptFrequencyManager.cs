namespace Authentication
{
    public class AuthenticationAttemptFrequencyManager:AttemptFrequencyManager<LastTriedToAuthenticate, long>
    {
        public AuthenticationAttemptFrequencyManager() {

        }
        public override bool TooFrequentlyTrying(long userId, out int delaySecondsRequired, long millisecondsNowUTC)
        {
            return base.TooFrequentlyTrying(userId, out delaySecondsRequired, millisecondsNowUTC);
        }
        public override void AddTry(long userId, long millisecondsNowUTC)
        {
            base.AddTry(userId, millisecondsNowUTC);
        }
        public override void Remove(long userId)
        {
            base.Remove(userId);
        }
    }
}

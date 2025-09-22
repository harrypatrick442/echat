using Core.Limiters;

namespace Authentication
{
    public class AttemptFrequencyManager<TLastTried, TIdentifier> where TLastTried:LastTriedBase
    {
        private Dictionary<TIdentifier, LastTriedToAuthenticate> _MapUserIdToLastAttemptsToAuthenticate = new Dictionary<TIdentifier, LastTriedToAuthenticate>();
        protected AttemptFrequencyManager() {

        }
        public virtual bool TooFrequentlyTrying(TIdentifier identifier, out int delaySecondsRequired, long millisecondsNowUTC) {
            delaySecondsRequired = 0;
            lock (_MapUserIdToLastAttemptsToAuthenticate) {
                if (!_MapUserIdToLastAttemptsToAuthenticate.ContainsKey(identifier))
                    return false;
                LastTriedToAuthenticate lastTriedToAuthenticate = _MapUserIdToLastAttemptsToAuthenticate[identifier];
                return !lastTriedToAuthenticate.CanDoAgainNow(millisecondsNowUTC, out delaySecondsRequired);
            }
        }
        public virtual void AddTry(TIdentifier identifier, long millisecondsNowUTC) {

            lock (_MapUserIdToLastAttemptsToAuthenticate)
            {
                if (!_MapUserIdToLastAttemptsToAuthenticate.ContainsKey(identifier)) {
                    _MapUserIdToLastAttemptsToAuthenticate.Add(identifier, new LastTriedToAuthenticate(millisecondsNowUTC));
                    return;
                }
                LastTriedToAuthenticate lastTriedToAuthenticate = _MapUserIdToLastAttemptsToAuthenticate[identifier];
                lastTriedToAuthenticate.Tried(millisecondsNowUTC);
            }
        }
        public virtual void Remove(TIdentifier identifier){
            lock (_MapUserIdToLastAttemptsToAuthenticate)
            {
                _MapUserIdToLastAttemptsToAuthenticate.Remove(identifier);
            }
        }
    }
}

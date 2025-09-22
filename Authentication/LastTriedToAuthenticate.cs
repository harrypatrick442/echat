using Core.Limiters;

namespace Authentication
{
    public class LastTriedToAuthenticate:LastTriedBase
    {
        private const int N_MINUTES_WINDOW = 1;
        private const int N_MILLISECONDS_WINDOW = N_MINUTES_WINDOW * 60000;
        private const int MAX_N_RECENT_TO_ALLOW_TRY_AGAIN = 2;
        public LastTriedToAuthenticate(long millisecondsUTCTriedAt):base(millisecondsUTCTriedAt, N_MILLISECONDS_WINDOW, MAX_N_RECENT_TO_ALLOW_TRY_AGAIN){
        
        }
    }
}

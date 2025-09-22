
using Users.Enums;

namespace Users.Users
{
    public class InviteException : Exception
    {
        private InviteFailedReason _FailedReason;
        public InviteFailedReason FailedReason {
            get{return _FailedReason;} 
            protected set{ _FailedReason= value ;}
        }
        public InviteException(InviteFailedReason failedReason) : base() {
            _FailedReason = failedReason;
        }
    }
}
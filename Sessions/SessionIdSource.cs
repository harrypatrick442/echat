using Core.Exceptions;
using Shutdown;
using Logging;
using Core.Ids;
using DependencyManagement;

namespace Sessions
{
    public class SessionIdSource:RobustEfficientIdSource, ISessionIdSource
    {
        private static SessionIdSource _Instance;
        public static SessionIdSource Instance {
            get
            {
                if (_Instance == null) 
                    throw new NotInitializedException(nameof(SessionIdSource));
                return _Instance; 
            } 
        }
        public static SessionIdSource Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(SessionIdSource));
            _Instance = new SessionIdSource();
            return _Instance;
        }
        private SessionIdSource() : base(1, DependencyManager.GetString(DependencyNames.SessionIdSourceDirectory),
            "sessionId")
        {
            LoadCurrentId();
            CheckSanity();
        }
        protected override long IncrementId(long currentId, long currentIdSanityCheck, out long newCurrentIdSanityCheck)
        {
            //Already locked from base class so be careful.
            try
            {
                newCurrentIdSanityCheck = currentIdSanityCheck + 1;
                long newCurrentId = currentId + 1;
                if (newCurrentId != newCurrentIdSanityCheck)
                    throw new FatalException("The new currentId's did not match");
                return newCurrentId;
            }
            catch (Exception ex)
            {
                Logs.HighPriority.Error(ex);
                _ShuttingDown = true;
                ShutdownManager.Instance.Shutdown(exitCode: 2);
                throw new FatalException("Failed to get the next session id", ex);
            }

        }
        protected override long IncrementIdBy(long currentId, long currentIdSanityCheck, int n, int nSanityCheck, out long newCurrentIdSanityCheck)
        {
            //Already locked from base class so be careful.
            try
            {
                newCurrentIdSanityCheck = currentIdSanityCheck + nSanityCheck;
                long newCurrentId = currentId + n;
                if (newCurrentId != newCurrentIdSanityCheck)
                    throw new FatalException("The new currentId's did not match");
                return newCurrentId;
            }
            catch (Exception ex)
            {
                Logs.HighPriority.Error(ex);
                _ShuttingDown = true;
                ShutdownManager.Instance.Shutdown(exitCode: 2);
                throw new FatalException("Failed to get the next session id", ex);
            }

        }
    }
}
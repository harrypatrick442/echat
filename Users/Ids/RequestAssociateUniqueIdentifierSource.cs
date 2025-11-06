using Shutdown;
using Core.Exceptions;
using Logging;
using Nodes;
using NodeAssignedIdRanges;
using DependencyManagement;
using Core.Ids;
using Initialization.Exceptions;

namespace Users
{
    public sealed class RequestAssociateUniqueIdentifierSource : RobustEfficientIdSource
    {
        private static RequestAssociateUniqueIdentifierSource _Instance;
        public static RequestAssociateUniqueIdentifierSource Instance { get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(RequestAssociateUniqueIdentifierSource)); 
                return _Instance; } }
        private NodeIdRanges _CurrentNodeIdRange, _CurrentNodeIdRangeSanityCheck;
        private INode _NodeMe;
        public static RequestAssociateUniqueIdentifierSource Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(RequestAssociateUniqueIdentifierSource));
            _Instance = new RequestAssociateUniqueIdentifierSource();
            return _Instance;
        }
        private RequestAssociateUniqueIdentifierSource() : base(nFiles: 3, DependencyManager.GetString(DependencyNames.RequestUniqueIdentifierSourceDirectory),
            "currentId")
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
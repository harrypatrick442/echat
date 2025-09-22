using Shutdown;
using Nodes;
using Core.Exceptions;
using Logging;
using NodeAssignedIdRanges;

namespace Core.Ids
{
    public class NodeAssignedIdSource : RobustEfficientIdSource
    {
        //CHECKED
        private IdRange _CurrentNodeIdRange, _CurrentNodeIdRangeSanityCheck;
        private MyIdRangesForIdType _MyIdRangesForIdTypeHelper;
        public NodeAssignedIdSource(
            string directoryPath, int idType) 
            : base(nFiles:3, directoryPath,
            "currentId")
        {
            _MyIdRangesForIdTypeHelper = NodesIdRangesManager.Instance.MineForIdType(idType);
            LoadCurrentId();
            CheckSanity();
        }
        protected override long IncrementIdBy(long currentId, long currentIdSanityCheck, int n, int nSanityCheck, out long newCurrentIdSanityCheck)
        {
            //Already locked from base class so be careful.
            try
            {
                if (_CurrentNodeIdRange == null)
                {
                    if (_CurrentNodeIdRangeSanityCheck != null)
                        throw new FatalException($"Had {nameof(_CurrentNodeIdRange)} but no {nameof(_CurrentNodeIdRangeSanityCheck)}");
                    _CurrentNodeIdRange = _MyIdRangesForIdTypeHelper.GetNodeIdRangeContainingOrGreaterThanId(currentId);
                    _CurrentNodeIdRangeSanityCheck = _MyIdRangesForIdTypeHelper.GetNodeIdRangeContainingOrGreaterThanId(currentIdSanityCheck);
                    if (_CurrentNodeIdRange != _CurrentNodeIdRangeSanityCheck)
                        throw new FatalException("The node id ranges returned were different");
                }
                else
                {
                    if (_CurrentNodeIdRangeSanityCheck == null)
                        throw new FatalException($"Had {nameof(_CurrentNodeIdRangeSanityCheck)} but no {nameof(_CurrentNodeIdRange)}");
                }
                long newCurrentId = IncrementIdBy(ref _CurrentNodeIdRange, currentId, n);
                newCurrentIdSanityCheck = IncrementIdBy(ref _CurrentNodeIdRangeSanityCheck, currentIdSanityCheck, nSanityCheck);
                if (newCurrentId != newCurrentIdSanityCheck)
                    throw new FatalException("The new currentId's did not match");
                return newCurrentId;
            }
            catch (Exception ex)
            {
                Logs.HighPriority.Error(ex);
                _ShuttingDown = true;
                ShutdownManager.Instance.Shutdown(exitCode: 2);
                throw new FatalException("Failed to get the next snippet id", ex);
            }

        }
        protected override long IncrementId(long currentId, long currentIdSanityCheck, 
            out long newCurrentIdSanityCheck)
        {
            //Already locked from base class so be careful.
            try
            {
                if (_CurrentNodeIdRange == null)
                {
                    if (_CurrentNodeIdRangeSanityCheck != null)
                        throw new FatalException($"Had {nameof(_CurrentNodeIdRange)} but no {nameof(_CurrentNodeIdRangeSanityCheck)}");
                    _CurrentNodeIdRange = _MyIdRangesForIdTypeHelper.GetNodeIdRangeContainingOrGreaterThanId(currentId);
                    _CurrentNodeIdRangeSanityCheck = _MyIdRangesForIdTypeHelper.GetNodeIdRangeContainingOrGreaterThanId(currentIdSanityCheck);
                    if (_CurrentNodeIdRange != _CurrentNodeIdRangeSanityCheck)
                        throw new FatalException("The node id ranges returned were different");
                }
                else
                {
                    if (_CurrentNodeIdRangeSanityCheck == null)
                        throw new FatalException($"Had {nameof(_CurrentNodeIdRangeSanityCheck)} but no {nameof(_CurrentNodeIdRange)}");
                }
                long newCurrentId = IncrementId(ref _CurrentNodeIdRange, currentId);
                newCurrentIdSanityCheck = IncrementId(ref _CurrentNodeIdRangeSanityCheck, 
                    currentIdSanityCheck);
                if(newCurrentId!=newCurrentIdSanityCheck)
                    throw new FatalException("The new currentId's did not match");
                return newCurrentId;
            }
            catch (Exception ex) {
                Logs.HighPriority.Error(ex);
                _ShuttingDown = true;
                ShutdownManager.Instance.Shutdown(exitCode:2);
                throw new FatalException("Failed to get the next id", ex);
            }

        }
        private long IncrementIdBy(ref IdRange currentNodeIdRange, long currentId, int n)
        {

            long nIncrementsLeftToDo = n;
            long idAt = currentId > currentNodeIdRange.FromInclusive
                ? currentId : currentNodeIdRange.FromInclusive;
            while (true)
            {

                long nIncrementsLeftToBeDoneInThisIdRange =
                    currentNodeIdRange.ToExclusive - (1 + idAt);
                if (nIncrementsLeftToBeDoneInThisIdRange >= nIncrementsLeftToDo)
                {
                    currentId = idAt + nIncrementsLeftToDo;
                    return currentId;
                }
                nIncrementsLeftToDo = nIncrementsLeftToDo - (nIncrementsLeftToBeDoneInThisIdRange + 1);
                currentNodeIdRange = _MyIdRangesForIdTypeHelper.GetNodeIdRangeContainingOrGreaterThanId(
                    currentNodeIdRange.ToExclusive);
                idAt = currentNodeIdRange.FromInclusive;
            }
        }
        private long IncrementId(ref IdRange currentNodeIdRange, long currentId) {
            long nextId = (currentId > currentNodeIdRange.FromInclusive
                ? currentId : currentNodeIdRange.FromInclusive)+1;
            if (nextId < currentNodeIdRange.ToExclusive)
            {
                return nextId;
            }
            currentNodeIdRange = _MyIdRangesForIdTypeHelper.GetNodeIdRangeContainingOrGreaterThanId(currentNodeIdRange.ToExclusive);
            nextId = currentNodeIdRange.FromInclusive;
            return nextId;
        }
    }
}
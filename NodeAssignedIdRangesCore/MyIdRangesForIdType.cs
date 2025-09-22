using Core.Exceptions;
using Logging;
using Shutdown;

namespace NodeAssignedIdRanges
{
    public class MyIdRangesForIdType
    {
        //CHECKED
        private NodesIdRangesForIdTypeManager _NodesIdRangesForIdTypeManager;
        private int _TypeId;
        public MyIdRangesForIdType(int typeId, NodesIdRangesForIdTypeManager nodesIdRangesForIdTypeManager) {
            _TypeId = typeId;
            _NodesIdRangesForIdTypeManager = nodesIdRangesForIdTypeManager;
        }
        public IdRange GetNodeIdRangeContainingOrGreaterThanId(long currentId) {
            NodeIdRangePair[] myRanges = _NodesIdRangesForIdTypeManager.MyNodeIdRangePairsOredered;
            int i = myRanges.Length - 1;
            if (i < 0)
            {
                return _NodesIdRangesForIdTypeManager.GetMyselfANewIdRange(0);
            }
            IdRange idRange = myRanges[i].IdRange;
            if (currentId>=idRange.FromInclusive){
                if (currentId >= idRange.ToExclusive)
                {
                    //Backup because ideally new range is already sorted beforehand
                    idRange = _NodesIdRangesForIdTypeManager.GetMyselfANewIdRange(idRange.ToExclusive);
                    if (idRange.ToExclusive <= currentId)
                    {
                        var fEx = new FatalException($"Id was much bigger than it should have been? Something went very wrong. {nameof(currentId)} was {currentId} but {nameof(idRange.ToExclusive)} was {idRange.ToExclusive }. {nameof(_TypeId)} was {_TypeId}");
                        Logs.HighPriority.Error(fEx);
                        ShutdownManager.Instance.Shutdown(exitCode: 2);
                        throw fEx;
                    }
                    return idRange;
                }
                return idRange;
            }
            IdRange currentIdRange = idRange;
            i--;
            while (i >= 0) { 
                IdRange nextIdRange= myRanges[i].IdRange;
                if (currentId >= nextIdRange.ToExclusive) {
                    return currentIdRange;
                }
                currentIdRange = nextIdRange;
                i--;
            }
            return currentIdRange;
        }
    }
}

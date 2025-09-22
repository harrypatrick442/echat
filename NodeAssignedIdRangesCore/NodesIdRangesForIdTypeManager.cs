using Core.Exceptions;
using Logging;
using Nodes;
using Shutdown;

namespace NodeAssignedIdRanges
{
    public class NodesIdRangesForIdTypeManager
    {
        private LinkedList<NodeIdRangePair> _MasterRecord
            = new LinkedList<NodeIdRangePair>();
        private volatile NodeIdRangePair[] _NodeIdRangePairsOrdered;
        private volatile NodeIdRangePair[] _MyNodeIdRangePairsOrdered;
        private MyIdRangesForIdType _MyIdRangesForIdType;
        public MyIdRangesForIdType MyIdRangesForIdType { get { return _MyIdRangesForIdType; } }
        public NodeIdRangePair[] MyNodeIdRangePairsOredered { get { return _MyNodeIdRangePairsOrdered; } }
        private readonly object _LockObjectGetMyselfANewIdRange = new object();
        public int IdType { get; }
        public int[] AllNodeIds { get; }
        private int _MyNodeId;
        public NodesIdRangesForIdTypeManager(int idType, NodeIdRanges[] nodeIdRangess)
        {
            IdType = idType;
            AllNodeIds = nodeIdRangess.Select(n => n.NodeId).GroupBy(i=>i)
                .Select(g => g.First()).ToArray();
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _NodeIdRangePairsOrdered = nodeIdRangess
                .SelectMany(n => n.IdRanges
                    .Select(i => new NodeIdRangePair(n.NodeId, i)))
                .OrderBy(p => p.IdRange.FromInclusive)
                .ToArray();
            _MyNodeIdRangePairsOrdered = _NodeIdRangePairsOrdered
                .Where(n => n.NodeId == _MyNodeId)
                .ToArray();
            _MasterRecord = new LinkedList<NodeIdRangePair>(_NodeIdRangePairsOrdered);
            _MyIdRangesForIdType = new MyIdRangesForIdType(idType, this);
        }
        public IdRange GetMyselfANewIdRange(long currentToExclusive) {
            lock (_LockObjectGetMyselfANewIdRange)
            {
                NodeIdRangePair? myLastNodeIdRangePair = _MyNodeIdRangePairsOrdered.LastOrDefault();
                if (myLastNodeIdRangePair != null) {
                    if (myLastNodeIdRangePair.IdRange.ToExclusive > currentToExclusive) {
                        return myLastNodeIdRangePair.IdRange;
                    }
                }
                IdRange idRange = IdRangesMesh.Instance.GiveMeNewIdRange(IdType);
                InsertNewNodeIdRangePair(new NodeIdRangePair(_MyNodeId, idRange));
                return idRange;
            }
         }
        internal void AnotherNodeGotNewIdRange(int nodeId, IdRange range)
        {
            InsertNewNodeIdRangePair(new NodeIdRangePair(nodeId, range));
        }
        public INode GetNodeForIdInRange(long id)
        {
            return Nodes.Nodes.Instance.GetNodeById(GetNodeIdForIdInRange(id));
        }
        public int GetNodeIdForIdInRange(long id)
        {
            NodeIdRangePair[] nodeIdRangePairs = _NodeIdRangePairsOrdered;
            if (nodeIdRangePairs == null || nodeIdRangePairs.Length < 1)//TODO make sure it creates first range if there is none
                throw DoFatalException($"{nameof(nodeIdRangePairs)} was empty. May not have initialized properly but should have shut down");
            int index = nodeIdRangePairs.Length - 1;
            while (index >= 0) {
                NodeIdRangePair nodeIdRangePair = nodeIdRangePairs[index];
                if (id >= nodeIdRangePair.IdRange.FromInclusive) {
                    if (id >= nodeIdRangePair.IdRange.ToExclusive) {
                        throw new ArgumentException($"Something went very wrong. The o {id} did not exist in any of the {nameof(NodeIdRangePair)}'s for {nameof(IdType)} {IdType}");
                    }
                    return nodeIdRangePair.NodeId;
                }
                index--;
            }
            throw new ArgumentException($"Something went very wrong. The o {id} did not exist in any of the {nameof(NodeIdRangePair)}'s for {nameof(IdType)} {IdType}");
        }
        public IEnumerable<NodeIdAndAssociattedObjects<TObject>>
            GetNodeIdAndAssociatedObjects_s<TObject>(
            IEnumerable<TObject> obs, Func<TObject, long> getObjectIdentifier, List<long>? missingIds = null) {
            TObject[] objects = obs.OrderByDescending(o => getObjectIdentifier(o)).ToArray();
            int objectsLength = objects.Count();
            if (objectsLength < 1)
            {
                yield break;
            }
            NodeIdRangePair[] nodeIdRangePairsOrdered = _NodeIdRangePairsOrdered;
            int n = nodeIdRangePairsOrdered.Length - 1;
            NodeIdRangePair nodeIdRangePair = nodeIdRangePairsOrdered[n];
            int i = 0;
            TObject o = objects[i];
            List<TObject> currentObjects = new List<TObject>();
            while (true)
            {
                IdRange idRange = nodeIdRangePair.IdRange;
                long identifier;
                while ((identifier = getObjectIdentifier(o)) >= idRange.ToExclusive)
                {
                    missingIds?.Add(identifier);
                    i++;
                    if (i >= objectsLength)
                    {
                        yield break;
                    }
                    o = objects[i];
                }
                while (getObjectIdentifier(o) >= idRange.FromInclusive)
                {
                    currentObjects.Add(o);
                    i++;
                    if (i >= objectsLength)
                    {
                        yield return new NodeIdAndAssociattedObjects<TObject>(nodeIdRangePair.NodeId, currentObjects.ToArray());
                        currentObjects.Clear();
                        yield break;
                    }
                    o = objects[i];
                }
                if (currentObjects.Any())
                {
                    yield return new NodeIdAndAssociattedObjects<TObject>(nodeIdRangePair.NodeId, currentObjects.ToArray());
                    currentObjects.Clear();
                }
                n--;
                if (n < 0)
                {
                    break;
                }
                nodeIdRangePair = nodeIdRangePairsOrdered[n];
            }
            if (currentObjects.Any())
            {
                yield return new NodeIdAndAssociattedObjects<TObject>(nodeIdRangePair.NodeId, currentObjects.ToArray());
                currentObjects.Clear();
            }
        }
        public List<NodeIdAndAssociatedIds> GetNodeIdsForIdsInRange(long[] ids, List<long> missingIds = null)
        {
            int idsLength = ids.Length;
            if (idsLength < 1)
            {
                return new List<NodeIdAndAssociatedIds>();
            }
            ids = ids.OrderByDescending(i => i).ToArray();
            NodeIdRangePair[] nodeIdRangePairsOrdered = _NodeIdRangePairsOrdered;
            int n = nodeIdRangePairsOrdered.Length - 1;
            NodeIdRangePair nodeIdRangePair = nodeIdRangePairsOrdered[n];
            int i = 0;
            long id = ids[i];
            List<NodeIdAndAssociatedIds> nodeIdAndAssociatedIds = new List<NodeIdAndAssociatedIds>();
            List<long> currentIds = new List<long>();
            Action doneWithThisRange = () =>
            {
                nodeIdAndAssociatedIds.Add(new NodeIdAndAssociatedIds(nodeIdRangePair.NodeId, currentIds.ToArray()));
                currentIds.Clear();
            };
            while (true)
            {
                IdRange idRange = nodeIdRangePair.IdRange;
                while (id >= idRange.ToExclusive)
                {
                    missingIds?.Add(id);
                    i++;
                    if (i >= idsLength)
                    {
                        return nodeIdAndAssociatedIds;
                    }
                    id = ids[i];
                }
                while (id >= idRange.FromInclusive)
                {
                    currentIds.Add(id);
                    i++;
                    if (i >= idsLength)
                    {
                        doneWithThisRange();
                        return nodeIdAndAssociatedIds;
                    }
                    id = ids[i];
                }
                if (currentIds.Any())
                {
                    doneWithThisRange();
                }
                n--;
                if (n < 0)
                {
                    break;
                }
                nodeIdRangePair = nodeIdRangePairsOrdered[n];
            }
            if (currentIds.Any())
            {
                doneWithThisRange();
            }
            return nodeIdAndAssociatedIds;
        }
        public NodeAndAssociatedIds[] GetNodesForIdsInRange(long[] ids, List<long> missingIds = null)
        {
            return GetNodeIdsForIdsInRange(ids, missingIds)
                .Select(n => new NodeAndAssociatedIds(Nodes.Nodes.Instance.GetNodeById(n.NodeId), n.Ids))
                .ToArray();
        }
        private void InsertNewNodeIdRangePair(NodeIdRangePair newNodeIdRangePair)
        {
            lock (_MasterRecord)
            {
                LinkedListNode<NodeIdRangePair>? existingNodeIdRangePair = _MasterRecord.Last;
                while (true)
                {
                    if (existingNodeIdRangePair == null)
                    {
                        _MasterRecord.AddFirst(newNodeIdRangePair);
                        break;
                    }
                    IdRange existingIdRange = existingNodeIdRangePair!.Value.IdRange;
                    IdRange newIdRange = newNodeIdRangePair.IdRange;
                    if (newIdRange.FromInclusive >=
                        existingIdRange.ToExclusive)
                    {
                        _MasterRecord.AddAfter(existingNodeIdRangePair, newNodeIdRangePair);
                        break;
                    }
                    if (newIdRange.ToExclusive> existingIdRange.FromInclusive) {
                        bool isAlreadyInserted = (newIdRange.FromInclusive == existingIdRange.FromInclusive)
                            && (newIdRange.ToExclusive == existingIdRange.ToExclusive);
                        if (isAlreadyInserted) {
                            return;
                        }
                        throw DoFatalException($"Something went very wrong. Id ranges overlapped for {nameof(IdType)} {IdType} with {nameof(newIdRange)} {newIdRange.FromInclusive}-{newIdRange.ToExclusive} overlapping {existingIdRange} {existingIdRange.FromInclusive}-{existingIdRange.ToExclusive}");
                    }
                    existingNodeIdRangePair = existingNodeIdRangePair.Previous;
                }
                _NodeIdRangePairsOrdered = _MasterRecord.ToArray();
                _MyNodeIdRangePairsOrdered = _NodeIdRangePairsOrdered
                    .Where(n => n.NodeId == _MyNodeId)
                    .ToArray();
            }
        }
        private static FatalException DoFatalException(string message) {
            FatalException ex = new FatalException(message);
            Logs.HighPriority.Error(ex);
            ShutdownManager.Instance.Shutdown(exitCode: 2);
            return ex;
        }
        /*
            ids = ids.OrderByDescending(id => id).ToArray(); 
            NodeIdRangePair[] nodeIdRangePairs = _IdRangeNodePairsOrdered;
            if (nodeIdRangePairs == null || nodeIdRangePairs.Length < 1)
                throw new FatalException($"{nameof(_IdRangeNodePairsOrdered)} was empty");
            int index = nodeIdRangePairs.Length - 1;
            while (index >= 0)
            {
                NodeIdRangePair nodeIdRangePair = nodeIdRangePairs[index];
                if (id >= nodeIdRangePair.IdRange.FromInclusive)
                {
                    if (id >= nodeIdRangePair.IdRange.ToExclusive)
                    {
                        throw new FatalException($"Something went very wrong. The id {id} did not exist in any of the {nameof(NodeIdRangePair)}'s for {nameof(IdType)} {IdType}");
                    }
                    return nodeIdRangePair.NodeId;
                }
            }
            throw new FatalException($"Something went very wrong. The id {id} did not exist in any of the {nameof(NodeIdRangePair)}'s for {nameof(IdType)} {IdType}");
        }
        private void Get_MapNodeIdToId_GetNodeAndAssociatedIds(out Action<int, long> mapNodeIdToId, out Func<NodeAndAssociatedIds[]> getNodeAndAssociatedIds)
        {
            Dictionary<int, List<long>> mapNodeIdToIds = new Dictionary<int, List<long>>();

            getNodeAndAssociatedIds = () => mapNodeIdToIds
                    .Select(p => new NodeAndAssociatedIds(Nodes.Nodes.Instance.GetNodeById(p.Key), p.Value.ToArray())).ToArray();
        }*/
    }
}

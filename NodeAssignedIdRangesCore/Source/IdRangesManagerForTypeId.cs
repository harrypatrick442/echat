using Core.Exceptions;
using KeyValuePairDatabases.Enums;
using KeyValuePairDatabases;
using NodeAssignedIdRangesSource.Serializables;
using DependencyManagement;

namespace NodeAssignedIdRanges
{
    public class IdRangesManagerForTypeId : IIdRangesManagerForIdType
    {
        //CHECKED
        public int IdType { get; }
        private string _DatabaseDirectoryPath;
        private KeyValuePairDatabase<long, IdRangesAssignedToNode> _IdRangesAssignedToANodeForIdTypeKeyValuePairDatabase;
        private KeyValuePairDatabase<long, NextIdFromForIdType> _NextIdFromForIdTypeKeyValuePairDatabase;
        public IdRangesManagerForTypeId(int idType, KeyValuePairDatabase<long, NextIdFromForIdType> nextIdFromForIdTypeKeyValuePairDatabase)
        {
            IdType = idType;
            _NextIdFromForIdTypeKeyValuePairDatabase = nextIdFromForIdTypeKeyValuePairDatabase;
            _DatabaseDirectoryPath = Path.Combine(DependencyManager.GetString(DependencyNames.IdRangesAssignedToNodesDatabaseDirectory), $"type_{idType}");
            _IdRangesAssignedToANodeForIdTypeKeyValuePairDatabase =
                new KeyValuePairDatabase<long, IdRangesAssignedToNode>(
                    OnDiskDatabaseType.Sqlite,
                    new OnDiskDatabaseParams
                    {
                        RootDirectory = _DatabaseDirectoryPath,
                        NCharactersEachLevel = 4,
                        Extension = ".json"
                    }, new IdentifierLock<long>()
            );
        }
        public NodeIdRanges[] GetIdRangesAssignedToNodes()
        {
            List<NodeIdRanges> list = new List<NodeIdRanges>();
            foreach (int otherNodesId in GlobalConstants.Nodes.GetNodeIdsAssociatedWithIdType(IdType))
            {
                IdRangesAssignedToNode idRangesAssignedToNode =
                    _IdRangesAssignedToANodeForIdTypeKeyValuePairDatabase.Get(otherNodesId);
                if (idRangesAssignedToNode == null)
                {
                    IdRange idRange = AssignNextIdRangeToNode(otherNodesId);
                    list.Add(new NodeIdRanges(otherNodesId, new IdRange[] { idRange }));
                    continue;
                }
                if (idRangesAssignedToNode.NodeId != otherNodesId)
                    throw new FatalException($"Something is wrong in {nameof(KeyValuePairDatabase<long, IdRangesAssignedToNode>)} with path \"{_DatabaseDirectoryPath}\". It seems one of the node id's changed");
                list.Add(new NodeIdRanges(otherNodesId, idRangesAssignedToNode.IdRanges));
            }
            return list.ToArray();
        }
        public IdRange AssignNextIdRangeToNode(int nodeId)
        {
            int sizeNewIdRangeShouldBe = SizeNewIdRangeForNodeShouldBe(nodeId);
            IdRange? newIdRange = null;
            _IdRangesAssignedToANodeForIdTypeKeyValuePairDatabase.ModifyWithinLock(nodeId, (idRangesAssignedToNode) =>
            {
                newIdRange = TakeNewIdRangeOfSize(sizeNewIdRangeShouldBe);
                if (idRangesAssignedToNode == null)
                {
                    idRangesAssignedToNode = new IdRangesAssignedToNode(nodeId, newIdRange);
                    return idRangesAssignedToNode;
                }
                idRangesAssignedToNode.Add(newIdRange);
                return idRangesAssignedToNode;
            });
            return newIdRange!;
        }
        private int SizeNewIdRangeForNodeShouldBe(int nodeId)
        {
#if DEBUG
            return 10;
#else
            return 10000;
#endif
        }
        private IdRange TakeNewIdRangeOfSize(int size)
        {
            IdRange? range = null;
            _NextIdFromForIdTypeKeyValuePairDatabase.ModifyWithinLock(IdType,
                (nextIdFromInclusiveForIdType) =>
            {
                if (nextIdFromInclusiveForIdType == null)
                {
                    nextIdFromInclusiveForIdType = new NextIdFromForIdType(1 + size);
                    range = new IdRange(1, nextIdFromInclusiveForIdType.Value);
                    return nextIdFromInclusiveForIdType;
                }
                long from = nextIdFromInclusiveForIdType.Value;
                nextIdFromInclusiveForIdType.Value = size + from;
                range = new IdRange(from, nextIdFromInclusiveForIdType.Value);
                return nextIdFromInclusiveForIdType;
            });
            return range!;
        }
    }
}
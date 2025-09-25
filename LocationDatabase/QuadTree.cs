using Core.Enums;
using Core.Exceptions;
using Core.Ids;
using Location.Interfaces;

namespace Location
{
    public abstract class QuadTree:IQuadTreeDatabase
    {
        protected IIdentifierToNodeId<long> _IdToNodeId;
        protected LevelQuadrantNodeMappings _LevelQuadrantNodeMappings;
        protected IQuadrantsLocalDatabase _QuadrantsLocalDatabase;
        protected ILevelQuadrantPairsForIdLocalDatabase _LevelQuadrantsParisForIdLocalDatabase;

        public DatabaseIdentifier Identifier { get; }
        public int NLevels => _LevelQuadrantNodeMappings.NLevels;
        public IQuadrantsLocalDatabase QuadrantsLocalDatabase
        { get {
                if (_QuadrantsLocalDatabase == null) {
                    throw new NotInitializedException($"No {nameof(IQuadrantsLocalDatabase)} was provided on this node. The node is probably not listed as a node provided to {nameof(LevelQuadrantNodeMappings)}");
                }
                return _QuadrantsLocalDatabase;
            }
        }
        public ILevelQuadrantPairsForIdLocalDatabase LevelQuadrantPairsForIdLocalDatabase
        {
            get
            {
                if (_LevelQuadrantsParisForIdLocalDatabase == null)
                {
                    throw new NotInitializedException($"No {nameof(LevelQuadrantsPairsForIdKeyValuePairDatabase)} was provided on this node.");
                }
                return _LevelQuadrantsParisForIdLocalDatabase;
            }
        }

        protected QuadTree(
            DatabaseIdentifier databaseIdentifier, IIdentifierToNodeId<long> idToNodeId, LevelQuadrantNodeMappings levelQuadrantNodeMappings) 
        {
            _IdToNodeId = idToNodeId;
            Identifier = databaseIdentifier;
            _LevelQuadrantNodeMappings = levelQuadrantNodeMappings;
            QuadTreeDatabasesInvolvedWithThisMachine.Register(this);
        }
        public int GetNodeIdForId(long id)
        {
            return _IdToNodeId.GetNodeIdFromIdentifier(id);
        }

        public int GetNodeIdForLevelAndQuadrant(int level, long quadrant)
        {
            return _LevelQuadrantNodeMappings.GetNodeId(level, quadrant);
        }
    }
}

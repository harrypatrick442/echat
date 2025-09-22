using Core.Enums;
using Core.Exceptions;
using DependencyManagement;
using LocationCore;
using LocationDatabase;
using Users;

namespace UserLocation
{
    public sealed class UserQuadTree:QuadTree
    {
        private static readonly int[][] NODES_AT_EACH_LEVEL =
#if DEBUG
            GlobalConstants.Nodes.USERS_QUAD_TREE_NODE_IDS_AT_EACH_LEVEL_DEBUG
#else
            GlobalConstants.Nodes.USERS_QUAD_TREE_NODE_IDS_AT_EACH_LEVEL
#endif
            ;
        private static UserQuadTree _Instance;
        public static UserQuadTree Instance { get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(UserQuadTree));
                return _Instance;
            }
        }

        public static UserQuadTree Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(UserQuadTree));
            _Instance = new UserQuadTree();
            return _Instance;
        }
        private UserQuadTree() : base(DatabaseIdentifier.UserQuadTree,
            UserIdToNodeId.Instance,
            new LevelQuadrantNodeMappings(
                NODES_AT_EACH_LEVEL
            ))
        {
            int myNodeId = Nodes.Nodes.Instance.MyId;
            bool hasLocalQuadrantsDatabase = NODES_AT_EACH_LEVEL.Where(nodesAtLevel => nodesAtLevel.Contains(myNodeId)).Any();
            if (hasLocalQuadrantsDatabase)
            {
                _QuadrantsLocalDatabase = new SqlLiteQuadrantsLocalDatabase(
                    DependencyManager.GetString(DependencyNames.UserQuadTreeQuadrantsDatabaseDirectory), _LevelQuadrantNodeMappings.NLevels);
            }
            bool hasLevelQuadrantsPairsForIdLocalDatabase = UserIdToNodeId.Instance.AllNodesIds.Contains(myNodeId);
            if (hasLevelQuadrantsPairsForIdLocalDatabase)
            {
                _LevelQuadrantsParisForIdLocalDatabase = new LevelQuadrantsPairsForIdKeyValuePairDatabase(
                    DependencyManager.GetString(DependencyNames.UserQuadTreeLevelQuadrantPairsForIdDatabaseDirectory));
            }
        }
        public void Set(long userId, LatLng latLng)
        {
            QuadTreeMesh.Instance.Set(Identifier, userId, latLng);
        }
        public void Delete(long userId)
        {
            QuadTreeMesh.Instance.Delete(Identifier, userId);
        }
        public Quadrant[] Get(LatLng latLng, double radiusKm) {
            return QuadTreeMesh.Instance.Get(Identifier, latLng, radiusKm, _LevelQuadrantNodeMappings.NLevels);
        }
        public Quadrant[] Get(LevelQuadrantPair[] levelQuadrantPairs)
        {
            return QuadTreeMesh.Instance.Get(Identifier, levelQuadrantPairs);
        }
        public QuadrantNEntries[] GetNEntries(int level, long[] quadrants, bool withLatLng)
        {
            return QuadTreeMesh.Instance.GetNEntries(Identifier, level, quadrants, withLatLng);
        }
    }
}
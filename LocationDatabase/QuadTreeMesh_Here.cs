using Core.Enums;
using Core.Threading;
using Location.Interfaces;
using LocationCore;
using System.Linq;

namespace Location
{
    public partial class QuadTreeMesh
    {
        private Quadrant[] GetIdsSpecificToNode_Here(DatabaseIdentifier databaseIdentifier,
            LevelQuadrantPair[] levelQuadrantPairs)
        {
            return QuadTreeDatabasesInvolvedWithThisMachine
                .Get(databaseIdentifier)
                .QuadrantsLocalDatabase
                .Get(levelQuadrantPairs);
        }
        private QuadrantNEntries[] GetNEntriesSpecificToNode_Here(DatabaseIdentifier databaseIdentifier,
            int level, long[] quadrants, bool withLatLng)
        {
            return QuadTreeDatabasesInvolvedWithThisMachine
                .Get(databaseIdentifier)
                .QuadrantsLocalDatabase
                .GetNEntries(level, quadrants, withLatLng);
        }
        /// <summary>
        /// On machine with IdLevelPairs        /// </summary>
        /// <param name="id"></param>
        /// <param name="latLng"></param>
        private void SetOnIdAssociatedNode_Here(DatabaseIdentifier databaseIdentifier, long id, LatLng latLng)
        {
            IQuadTreeDatabase quadTreeDatabase = QuadTreeDatabasesInvolvedWithThisMachine.Get(databaseIdentifier);
            LevelQuadrantPair[] newLevelQuadrantPairs = QuadrantsHelper.GetLevelQuadrantPairsForLatLng(latLng, quadTreeDatabase.NLevels);
            NodeIdAndLevelQuadrantPairs[] nodeIdAndLevelQuadrantPairss = GroupByNodeId(databaseIdentifier, newLevelQuadrantPairs);
            ILevelQuadrantPairsForIdLocalDatabase levelQuadrantPairsForIdDatabase = quadTreeDatabase.LevelQuadrantPairsForIdLocalDatabase;
            levelQuadrantPairsForIdDatabase.LockOnIdForWrite(id, () =>
            {
                LevelQuadrantPairsForId levelQuadrantPairsForId = levelQuadrantPairsForIdDatabase.Get(id);
                if (levelQuadrantPairsForId != null)
                {
                    ParallelOperationHelper.RunInParallelNoReturn<NodeIdAndLevelQuadrantPairs>(
                        GroupByNodeId(databaseIdentifier, levelQuadrantPairsForId.LevelQuadrantPairs),
                        (nodeIdAndLevelQuadrantPair) =>
                        {
                            DeleteSpecificToNode(nodeIdAndLevelQuadrantPair.NodeId, databaseIdentifier, id, 
                                nodeIdAndLevelQuadrantPair.LevelQuadrantPairs.Select(l => l.Level).ToArray());
                        },
                        GlobalConstants.Threading.MAX_N_THREADS_QUAD_TREE_DELETE,
                        throwExceptions: true);
                    levelQuadrantPairsForId.LevelQuadrantPairs = newLevelQuadrantPairs;
                }
                else
                {
                    levelQuadrantPairsForId = new LevelQuadrantPairsForId(newLevelQuadrantPairs);
                }
                levelQuadrantPairsForIdDatabase.Set(id, levelQuadrantPairsForId);
                ParallelOperationHelper.RunInParallelNoReturn<NodeIdAndLevelQuadrantPairs>(
                    nodeIdAndLevelQuadrantPairss,
                    (nodeIdAndLevelQuadrantPair) =>
                    {
                        SetSpecificToNode(databaseIdentifier, nodeIdAndLevelQuadrantPair.NodeId, 
                            id, latLng, nodeIdAndLevelQuadrantPair.LevelQuadrantPairs);
                    },
                    GlobalConstants.Threading.MAX_N_THREADS_QUAD_TREE_SET, throwExceptions:true);
            });
        }
        private void SetSpecificToNode_Here(DatabaseIdentifier databaseIdentifier,
            long id, LatLng latLng, LevelQuadrantPair[] levelQuadrantPairs) {
                QuadTreeDatabasesInvolvedWithThisMachine
                .Get(databaseIdentifier)
                .QuadrantsLocalDatabase
                .Set(id, latLng.Lat, latLng.Lng, levelQuadrantPairs);
        }

        private void DeleteOnIdAssociatedNode_Here(DatabaseIdentifier databaseIdentifier, long id)
        {
            IQuadTreeDatabase quadTreeDatabase = QuadTreeDatabasesInvolvedWithThisMachine.Get(databaseIdentifier);
            ILevelQuadrantPairsForIdLocalDatabase levelQuadrantPairsForIdDatabase = quadTreeDatabase.LevelQuadrantPairsForIdLocalDatabase;

            levelQuadrantPairsForIdDatabase.LockOnIdForWrite(id, () =>
            {
                LevelQuadrantPairsForId levelQuadrantPairsForId = levelQuadrantPairsForIdDatabase.Get(id);
                ParallelOperationHelper.RunInParallelNoReturn<NodeIdAndLevelQuadrantPairs>(
                    GroupByNodeId(databaseIdentifier, levelQuadrantPairsForId.LevelQuadrantPairs),
                    (nodeIdAndLevelQuadrantPair) =>
                    {
                        DeleteSpecificToNode(nodeIdAndLevelQuadrantPair.NodeId, 
                            databaseIdentifier, id,
                            nodeIdAndLevelQuadrantPair.LevelQuadrantPairs.Select(l => l.Level).ToArray());
                    },
                    GlobalConstants.Threading.MAX_N_THREADS_QUAD_TREE_DELETE,
                    throwExceptions: true);
                levelQuadrantPairsForIdDatabase.Set(id, null);
            });
        }
        private void DeleteSpecificToNode_Here(DatabaseIdentifier databaseIdentifier, long id, int[] levels)
        {
            QuadTreeDatabasesInvolvedWithThisMachine
                .Get(databaseIdentifier)
                .QuadrantsLocalDatabase
                .Delete(id, levels);
        }
    }
}

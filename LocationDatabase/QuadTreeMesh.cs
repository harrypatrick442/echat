using Core.Enums;
using Core.Exceptions;
using Core.Ids;
using Core.Threading;
using InterserverComs;
using Location.Requests;
using Location.Responses;
using LocationCore;
using Logging;
using Shutdown;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationDatabase
{
    public partial class QuadTreeMesh
    {
        private static QuadTreeMesh _Instance;
        public static QuadTreeMesh Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(QuadTreeMesh));
                return _Instance;
            }
        }

        public static QuadTreeMesh Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(QuadTreeMesh));
            _Instance = new QuadTreeMesh();
            return _Instance;
        }
        #region Public
        protected QuadTreeMesh()
        {
            Initialize_Server();
        }
        public Quadrant[] Get(DatabaseIdentifier databaseIdentifier, LatLng latLng, double radiusKm, int nLevels)
        {
            LevelQuadrantPair[] levelQuadrantPairs = QuadrantsHelper.GetQuadrantsForLatLngAndRadiusf(latLng, radiusKm, nLevels);
            return Get(databaseIdentifier, levelQuadrantPairs);
        }
        public Quadrant[] Get(DatabaseIdentifier databaseIdentifier, params LevelQuadrantPair[] levelQuadrantPairs)
        {
            IEnumerable<NodeIdAndLevelQuadrantPairs> nodeIdAndLevelQuadrantPairss = GroupByNodeId(
                    databaseIdentifier, levelQuadrantPairs);
            ParallelOperationResult<NodeIdAndLevelQuadrantPairs, Quadrant[]>[] results = ParallelOperationHelper
                .RunInParallel<NodeIdAndLevelQuadrantPairs, Quadrant[]>(
                nodeIdAndLevelQuadrantPairss,
                (nodeIdAndLevelQuadrantPair) =>
                {
                    return GetIdsSpecificToNode(databaseIdentifier, nodeIdAndLevelQuadrantPair.NodeId, nodeIdAndLevelQuadrantPair.LevelQuadrantPairs);
                },
                GlobalConstants.Threading.MAX_N_THREADS_QUAD_TREE_GET_IDS);
            Dictionary<long, Quadrant> mapIdToQuadrant = new Dictionary<long, Quadrant>();
            foreach (var result in results)
            {
                if (!result.Success)
                    continue;
                foreach (Quadrant quadrant in result.Return)
                {
                    if (mapIdToQuadrant.ContainsKey(quadrant.Id))
                        continue;
                    mapIdToQuadrant.Add(quadrant.Id, quadrant);
                }
            }
            return mapIdToQuadrant.Values.ToArray();
        }
        public QuadrantNEntries[] GetNEntries(DatabaseIdentifier databaseIdentifier, int level,
            long[] quadrants, bool withLatLng)
        {
            IEnumerable<NodeIdAndQuadrants> nodeIdAndQuadrantss = GroupByNodeId(
                    databaseIdentifier, level, quadrants);
            ParallelOperationResult<NodeIdAndQuadrants, QuadrantNEntries[]>[] results = ParallelOperationHelper
                .RunInParallel<NodeIdAndQuadrants, QuadrantNEntries[]>(
                nodeIdAndQuadrantss,
                (nodeIdAndQuadrants) =>
                {
                    return GetNEntriesSpecificToNode(databaseIdentifier, nodeIdAndQuadrants.NodeId, level,
                        nodeIdAndQuadrants.Quadrants, withLatLng);
                },
                GlobalConstants.Threading.MAX_N_THREADS_QUAD_TREE_GET_IDS);
            List<QuadrantNEntries> quadrantNEntriess = new List<QuadrantNEntries>();
            foreach (var result in results)
            {
                if (!result.Success)
                    continue;
                quadrantNEntriess.AddRange(result.Return);
            }
            return quadrantNEntriess.ToArray();
        }
        public void Set(DatabaseIdentifier databaseIdentifier, long id, LatLng latLng)
        {
            int nodeId = QuadTreeDatabasesInvolvedWithThisMachine.Get(databaseIdentifier)
                .GetNodeIdForId(id);
            try
            {
                OperationRedirectHelper.OperationRedirectedToNode<SetOnIdAssociatedNodeRequest, SetOnIdAssociatedNodeResponse>(nodeId,
                    callbackDoHere: () =>
                    {
                        SetOnIdAssociatedNode_Here(databaseIdentifier, id, latLng);
                    },
                    callbackCreateRequest: () => new SetOnIdAssociatedNodeRequest(databaseIdentifier, id, latLng),
                    didRemotely: (response) =>
                    {
                        if (!response.Successful)
                            throw new OperationFailedException(nameof(Set));
                    },
                    ShutdownManager.Instance.CancellationToken);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        public void Delete(DatabaseIdentifier databaseIdentifier, long id)
        {
            OperationRedirectHelper.OperationRedirectedToNode<DeleteOnIdAssociatedNodeRequest,
                DeleteOnIdAssociatedNodeResponse>(
            QuadTreeDatabasesInvolvedWithThisMachine.Get(databaseIdentifier).GetNodeIdForId(id),
            callbackDoHere: () =>
            {
                DeleteOnIdAssociatedNode_Here(databaseIdentifier, id);
            },
            callbackCreateRequest: () => new DeleteOnIdAssociatedNodeRequest(databaseIdentifier, id),
            didRemotely: (response) =>
            {
                if (!response.Successful)
                    throw new OperationFailedException(nameof(Delete));
            },
            ShutdownManager.Instance.CancellationToken);
        }
        #endregion Public
        #region Private
        private Quadrant[] GetIdsSpecificToNode(DatabaseIdentifier databaseIdentifier, int nodeId, LevelQuadrantPair[] levelQuadrantPairs)
        {
            Quadrant[] quadrants= null;
            OperationRedirectHelper.OperationRedirectedToNode<GetIdsSpecificToNodeRequest, GetIdsSpecificToNodeResponse>(nodeId,
                callbackDoHere: () =>
                {
                    quadrants = GetIdsSpecificToNode_Here(databaseIdentifier, levelQuadrantPairs);
                },
                callbackCreateRequest: () => new GetIdsSpecificToNodeRequest(databaseIdentifier, levelQuadrantPairs),
                didRemotely: (response) =>
                {
                    if (!response.Successful)
                        throw new OperationFailedException(nameof(GetIdsSpecificToNode));
                    quadrants = response.Quadrants;
                },
                ShutdownManager.Instance.CancellationToken);
            return quadrants;
        }
        private QuadrantNEntries[] GetNEntriesSpecificToNode(DatabaseIdentifier databaseIdentifier, 
            int nodeId, int level, long[] quadrants, bool withLatLng)
        {
            QuadrantNEntries[] quadrantNEntriess= null;
            OperationRedirectHelper.OperationRedirectedToNode<
                GetNEntriesSpecificToNodeRequest, GetNEntriesSpecificToNodeResponse>(nodeId,
                callbackDoHere: () =>
                {
                    quadrantNEntriess = GetNEntriesSpecificToNode_Here(databaseIdentifier, level, quadrants, withLatLng);
                },
                callbackCreateRequest: () => new GetNEntriesSpecificToNodeRequest(databaseIdentifier,
                    level, quadrants),
                didRemotely: (Action<GetNEntriesSpecificToNodeResponse>)((response) =>
                {
                    if (!response.Successful)
                        throw new OperationFailedException(nameof(GetNEntriesSpecificToNode));
                    quadrantNEntriess = response.QuadrantNEntriess;
                }),
                ShutdownManager.Instance.CancellationToken);
            return quadrantNEntriess;
        }
        private void SetSpecificToNode(DatabaseIdentifier databaseIdentifier, int nodeId, long id,
            LatLng latLng, LevelQuadrantPair[] levelQuadrantPairs)
        {
            OperationRedirectHelper.OperationRedirectedToNode<SetSpecificToNodeRequest, SetSpecificToNodeResponse>(
            nodeId,
            callbackDoHere: () =>
            {
                SetSpecificToNode_Here(databaseIdentifier, id, latLng,levelQuadrantPairs);
            },
            callbackCreateRequest: () => new SetSpecificToNodeRequest(databaseIdentifier, id, latLng, levelQuadrantPairs),
            didRemotely: (response) =>
            {
                if (!response.Successful)
                    throw new OperationFailedException(nameof(SetSpecificToNode));
            },
            ShutdownManager.Instance.CancellationToken);
        }
        private void DeleteSpecificToNode(int nodeId, DatabaseIdentifier databaseIdentifier, long id, int[] levels)
        {
            OperationRedirectHelper.OperationRedirectedToNode<DeleteSpecificToNodeRequest, DeleteSpecificToNodeResponse>(
            nodeId,
            callbackDoHere: () =>
            {
                DeleteSpecificToNode_Here(databaseIdentifier, id, levels);
            },
            callbackCreateRequest: () => new DeleteSpecificToNodeRequest(databaseIdentifier, id, levels),
            didRemotely: (response) =>
            {
                if (!response.Successful)
                    throw new OperationFailedException(nameof(DeleteSpecificToNode));
            },
            ShutdownManager.Instance.CancellationToken);
        }
        private NodeIdAndLevelQuadrantPairs[] GroupByNodeId(DatabaseIdentifier databaseIdentifier, IEnumerable<LevelQuadrantPair> levelQuadrantPairs)
        {
            return levelQuadrantPairs
                    .Select(l => new {
                        nodeId = QuadTreeDatabasesInvolvedWithThisMachine.Get(databaseIdentifier)
                        .GetNodeIdForLevelAndQuadrant(l.Level, l.Quadrant),
                        levelQuadrantPair = l
                    })
                    .GroupBy(o => o.nodeId)
                    .Select(g => new NodeIdAndLevelQuadrantPairs(
                                        g.First().nodeId, g.Select(o => o.levelQuadrantPair).ToArray()
                                 )
                   ).ToArray();
        }
        private NodeIdAndQuadrants[] GroupByNodeId(DatabaseIdentifier databaseIdentifier, int level, long[] quadrants)
        {
            return quadrants
                    .Select(q => new {
                        nodeId = QuadTreeDatabasesInvolvedWithThisMachine.Get(databaseIdentifier)
                        .GetNodeIdForLevelAndQuadrant(level, q),
                        quadrant = q
                    })
                    .GroupBy(o => o.nodeId)
                    .Select(g => new NodeIdAndQuadrants(
                                        g.First().nodeId, g.Select(o => o.quadrant).ToArray()
                                 )
                   ).ToArray();
        }
        #endregion Private
    }
}
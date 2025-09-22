using Core.Exceptions;
using Core.Messages.Responses;
using Core.Threading;
using HashTags.Enums;
using HashTags.Messages;
using InterserverComs;
using Logging;
using NodeAssignedIdRanges;
using Shutdown;
namespace HashTags
{
    public partial class HashTagsMesh
    {
        private static HashTagsMesh? _Instance;
        public static HashTagsMesh Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(HashTagsMesh));
            _Instance = new HashTagsMesh();
            return _Instance;
        }
        public static HashTagsMesh Instance { 
            get { 
                if (_Instance == null) throw new NotInitializedException(nameof(HashTagsMesh));
                return _Instance;
            } 
        }
        private long _MyNodeId;
        private NodesIdRangesForIdTypeManager _NodesIdRangesUsersManager;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private HashTagsMesh() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.HashTags);
            Initialize_Server();
        }
        #region Methods
        #region Public
        public bool SearchToPredictTag(
            string? str, HashTagScopeTypes? scopeType, int maxNEntries, out string[]? matches)
        {
            str = HashTagsHelper.NormalizeRemoveIllegalCharacters(str);
            if (str == null)
            {
                matches = null;
                return true;
            }
            bool success = true;
            string[]? matchesInternal = null;
            try
            {
                OperationRedirectHelper.OperationRedirectedToNode<SearchToPredictTagRequest, SearchToPredictTagResponse>(
                    HashTagNodeShardMappings.Instance.GetNodeId(str),
                    () => matchesInternal = SearchToPredictTag_Here(str, scopeType, maxNEntries),
                    () => new SearchToPredictTagRequest(str, scopeType, maxNEntries),
                    (response) =>
                    {
                        success = response.Success;
                        matchesInternal = response.Matches;
                    },
                    _CancellationTokenSourceDisposed.Token
                );
            }
            catch (Exception ex)
            {
                success = false;
                Logs.Default.Error(ex);
            }
            matches = matchesInternal;
            return success;
        }
        public SearchTagsResultForScopeType[] SearchTagsMultipleScopeTypes(
            string? tag, HashTagScopeTypes[] scopeTypes, bool allowPartialMatches, int maxNEntriesPerScopeType)
        {
            List<SearchTagsResultForScopeType> results = new List<SearchTagsResultForScopeType>(scopeTypes.Length);
            foreach (HashTagScopeTypes scopeType in scopeTypes) {
                try
                {
                    bool success = SearchTags(tag, scopeType, allowPartialMatches, maxNEntriesPerScopeType, out ScopeIds[]? exactMatches, out TagWithScopeIds[]? partialMatches);
                    results.Add(new SearchTagsResultForScopeType(success, scopeType, exactMatches, partialMatches));
                }
                catch (Exception ex) {
                    Logs.Default.Error(ex);
                    results.Add(new SearchTagsResultForScopeType(false, scopeType, null, null));
                }
            }
            return results.ToArray();
        }
        public bool SearchTags(string? tag, HashTagScopeTypes scopeType, bool allowPartialMatches,
            int maxNEntries, out ScopeIds[]? exactMatches, out TagWithScopeIds[]? partialMatches)
        {
            tag = HashTagsHelper.NormalizeRemoveIllegalCharacters(tag);
            if (tag==null)
            {
                exactMatches = null;
                partialMatches = null;
                return false;
            }
            if (maxNEntries > GlobalConstants.HashTags.MAX_N_SEARCH_ENTRIES) 
                maxNEntries = GlobalConstants.HashTags.MAX_N_SEARCH_ENTRIES;
            TagWithScopeIds[]? partialMatchesInternal = null;
            ScopeIds[]? exactMatchesInternal = null;
            bool success = true;
            try
            {
                OperationRedirectHelper.OperationRedirectedToNode<SearchTagsRequest, SearchTagsResponse>(
                    HashTagNodeShardMappings.Instance.GetNodeId(tag),
                    () => exactMatchesInternal = SearchTags_Here(tag, scopeType, allowPartialMatches, maxNEntries, out partialMatchesInternal),
                    () => new SearchTagsRequest(tag, scopeType, allowPartialMatches, maxNEntries),
                    (response) =>
                    {
                        success = response.Success;
                        exactMatchesInternal = response.ExactMatches;
                        partialMatchesInternal = response.PartialMatches;
                    },
                    _CancellationTokenSourceDisposed.Token
                );
            }
            catch (Exception ex)
            {
                success = false;
                Logs.Default.Error(ex);
            }
            partialMatches = partialMatchesInternal;
            exactMatches = exactMatchesInternal;
            return success;
        }
        public bool AddTags(string[] tagsUnfiltered, HashTagScopeTypes scopeType, long scopeId, long? scopeId2)
        {
            IEnumerable<NodeIdAndAssociatedTags> nodeIdAndAssociatedTagss = 
                GetNodeIdAndAssociatedTags(
                    HashTagsHelper.NormalizeRemoveIllegalCharactersAndRemoveDuplicates(
                        tagsUnfiltered
                )
            );
            bool success = true;
            ParallelOperationHelper.RunInParallelNoReturn(nodeIdAndAssociatedTagss, (nodeIdAndAssociatedTags) =>
            {
                try
                {
                    OperationRedirectHelper.OperationRedirectedToNode<AddTagsRequest, SuccessTicketedResponse>(
                        nodeIdAndAssociatedTags.NodeId,
                        () => AddTags_Here(nodeIdAndAssociatedTags.Tags, scopeType, scopeId, scopeId2),
                        () => new AddTagsRequest(nodeIdAndAssociatedTags.Tags, scopeType, scopeId, scopeId2),
                        (response) =>
                        {
                            success = success&&response.Success;
                        },
                        _CancellationTokenSourceDisposed.Token
                    );
                }
                catch (Exception ex)
                {
                    success = false;
                    Logs.Default.Error(ex);
                }
            },
            maxNThreads: GlobalConstants.Threading.MAX_N_THREADS_HASH_TAGS, 
            throwExceptions: false);
            return success;
        }
        public bool DeleteTags(HashTagScopeTypes scopeType, long scopeId, long? scopeId2, string[] tagsUnfiltered)
        {
            IEnumerable<NodeIdAndAssociatedTags> nodeIdAndAssociatedTagss =
                GetNodeIdAndAssociatedTags(
                    HashTagsHelper.NormalizeRemoveIllegalCharactersAndRemoveDuplicates(
                        tagsUnfiltered
                )
            );
            bool success = true;
            ParallelOperationHelper.RunInParallelNoReturn(nodeIdAndAssociatedTagss, (nodeIdAndAssociatedTags) =>
            {
                try
                {
                    OperationRedirectHelper.OperationRedirectedToNode<DeleteTagsRequest, SuccessTicketedResponse>(
                        nodeIdAndAssociatedTags.NodeId,
                        () => DeleteTags_Here(scopeType, scopeId, scopeId2, nodeIdAndAssociatedTags.Tags),
                        () => new DeleteTagsRequest(scopeType, scopeId, scopeId2, nodeIdAndAssociatedTags.Tags),
                        (response) =>
                        {
                            success = success && response.Success;
                        },
                        _CancellationTokenSourceDisposed.Token
                    );
                }
                catch (Exception ex)
                {
                    success = false;
                    Logs.Default.Error(ex);
                }
            },
            maxNThreads: GlobalConstants.Threading.MAX_N_THREADS_HASH_TAGS,
            throwExceptions: false);
            return success;
        }
        #endregion Public
        #region Private
        private IEnumerable<NodeIdAndAssociatedTags> GetNodeIdAndAssociatedTags(IEnumerable<string> filteredTags) {
            return filteredTags
                .Select(tag => new
                {
                    tag,
                    nodeId = HashTagNodeShardMappings.Instance.GetNodeId(tag)
                })
                .GroupBy(o => o.nodeId)
                .Select(g => 
                    new NodeIdAndAssociatedTags(
                        g.First().nodeId, 
                        g.Select(o => o.tag).ToArray()
                    )
                );
        }
        private void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
        }

        #endregion Private
#endregion Methods
    }
}
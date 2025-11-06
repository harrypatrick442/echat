using Core.Threading;
using Core.Exceptions;
using Logging;
using Shutdown;
using UserRouting;
using Users.Messages.Client;
using JSON;
using InterserverComs;
using Users.DAL;
using Initialization.Exceptions;

namespace Users.FrequentlyAccessedUserProfiles
{
    public partial class FrequentlyAccessedUserProfilesManager
    {
        private static FrequentlyAccessedUserProfilesManager _Instance;
        public static FrequentlyAccessedUserProfilesManager Initialize() {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(FrequentlyAccessedUserProfilesManager));
            _Instance = new FrequentlyAccessedUserProfilesManager();
            return _Instance;
        }
        public static FrequentlyAccessedUserProfilesManager Instance { 
            get { 
                if (_Instance == null) throw new NotInitializedException(nameof(FrequentlyAccessedUserProfilesManager));
                return _Instance;
            } 
        }
        private long _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private FrequentlyAccessedUserProfilesManager() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            DalFrequentlyAccessedUserProfiles.Initialize();
            Initialize_Server();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.FrequentlyAccessedUserProfilesManager);
        }
        #region Methods
        #region Public
        public FrequentlyAccessedUserProfile Get(long userId) {
            return DalFrequentlyAccessedUserProfiles.Instance.GetHereIfHasElseCoreAndCacheHere(userId);
        }
        public void Set(long userId, FrequentlyAccessedUserProfile frequentlyAccessedUserProfile) {
            DalFrequentlyAccessedUserProfiles.Instance.SetCoreAndHere(userId, frequentlyAccessedUserProfile);
            new Thread(() => _Set_PushToOtherNodes(userId, frequentlyAccessedUserProfile))
                .Start();
        }
        #endregion Public
        #region Private
        private void _Set_PushToOtherNodes(long userId, FrequentlyAccessedUserProfile frequentlyAccessedUserProfile) {
            try
            {
                NodeIdSessionIdPair[] nodeIdSessionIdPairs = CoreUserRoutingTable.Instance.GetNodeIdSessionIdPairs(userId);
                FrequentlyAccessedUserProfileUpdateRequest request =
                    new FrequentlyAccessedUserProfileUpdateRequest(userId, frequentlyAccessedUserProfile);
                string requestJsonString = Json.Serialize(request);
                ParallelOperationHelper.RunInParallel(nodeIdSessionIdPairs, (nodeIdSessionIdPair) =>
                {
                    try
                    {
                        if (nodeIdSessionIdPair.NodeId == _MyNodeId) return;
                        INodeEndpoint nodeEndpoint = InterserverPort.Instance.InterserverEndpoints.GetEndpoint(nodeIdSessionIdPair.NodeId);
                        nodeEndpoint.SendJSONString(requestJsonString);
                    }
                    catch (Exception ex)
                    {
                        Logs.Default.Error(ex);
                    }
                }, Configurations.Threading.MAX_N_THREADS_DISTRIBUTE_FREQUENTLY_ACCESSED_USER_PROFILE_UPDATE);
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
            }
        }
        private void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
        }
#endregion Private
#endregion Methods
    }
}
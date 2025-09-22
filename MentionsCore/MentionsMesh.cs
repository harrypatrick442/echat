using Core.Exceptions;
using Shutdown;
using InterserverComs;
using Logging;
using Core.Handlers;
using Core;
using MessageTypes.Internal;
using Core.Ids;
using MentionsCore.Messages;
using MentionsCore.Requests;
using MentionsCore.Responses;
using Users;
using Core.DAL;
using UserRoutedMessages;
using Core.Threading;
using NodeAssignedIdRanges;
using JSON;

namespace MentionsCore
{
    public sealed partial class MentionsMesh
    {
        private static MentionsMesh? _Instance;
        public static MentionsMesh Initialize() {
            if (_Instance != null) 
                throw new AlreadyInitializedException(nameof(MentionsMesh));
            _Instance = new MentionsMesh();
            return _Instance;
        }
        public static MentionsMesh Instance {
            get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(MentionsMesh));
                return _Instance;
            }
        }
        private int _MyNodeId;
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private IIdentifierToNodeId<long> _UserIdToNodeId;
        private MentionsIdSource _MentionsIdSource;
        private DalMentionsSQLite _DalMentionsSQLite;
        private NodesIdRangesForIdTypeManager _NodesIdRangesUsersManager;
        private MentionsMesh() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _MentionsIdSource = MentionsIdSource.Initialize();
            _NodesIdRangesUsersManager = NodesIdRangesManager.Instance.ForIdType(GlobalConstants.IdTypes.USER);
            _DalMentionsSQLite = DalMentionsSQLite.Initialize();
            _UserIdToNodeId = UserIdToNodeId.Instance;
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.MentionsAddOrUpdate, HandleAdd},
                    {InterserverMessageTypes.MentionsGet, HandleGet},
                    {InterserverMessageTypes.MentionsSetSeen, HandleSetSeen}
                }
            );
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.MentionsMesh);
        }
        #region Methods
        #region Public
        public bool Get(long userId, int nEntries, out Mention[]? mentions, long? idToExclusive, long? idFromInclusive)
        {
            Mention[]? mentionsInternal = null;
            bool success = true;
            try
            {
                OperationRedirectHelper.OperationRedirectedToNode<
                    GetMentionsRequest,
                    GetMentionsResponse>(
                    _UserIdToNodeId.GetNodeIdFromIdentifier(userId),
                    () =>
                    {
                        mentionsInternal =
                            Get_Here(userId, nEntries, idToExclusive, idFromInclusive);
                    },
                    () => new GetMentionsRequest(userId, nEntries, idToExclusive, idFromInclusive),
                    (response) =>
                    {
                        success = response.Successful;
                        mentionsInternal = response.Entries!;
                    },
                    _CancellationTokenSourceDisposed.Token
                );
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                success = false;
            }
            mentions = mentionsInternal;
            return success!;
        }
        public void Add(long[] userIdBeingMentioneds, Mention mention, bool isUpdate)
        {
            userIdBeingMentioneds = userIdBeingMentioneds.GroupBy(u => u).Select(g => g.First()).ToArray();
            List<long> missingIds = new List<long>();
            NodeAndAssociatedIds[] nodeAndAssociatedIdss = _NodesIdRangesUsersManager
                .GetNodesForIdsInRange(userIdBeingMentioneds, missingIds);
            if (missingIds.Count > 0)
            {
                Logs.Default.Error(new Exception("Missing ids "+string.Join(',', missingIds)));
            }
            if (nodeAndAssociatedIdss == null) return;
            string addMention = Json.Serialize(new AddOrUpdateMention(userIdBeingMentioneds, mention, isUpdate));
            ParallelOperationHelper.RunInParallelNoReturn(
                   nodeAndAssociatedIdss, (nodeAndAssociatedIds) => {
                       try
                       {
                           int nodeId = nodeAndAssociatedIds.Node.Id;
                           if (_MyNodeId == nodeId)
                           {
                               Add_Here(nodeAndAssociatedIds.Ids, mention, isUpdate);
                               return;
                           }
                           INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(nodeId);
                           nodeEndpoint.SendJSONString(addMention);
                       }
                       catch (Exception ex) {
                           Logs.Default.Error(ex);
                       }
                   }, GlobalConstants.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_CORE_SERVERS_FOR_USER);
            UserRoutedMessagesManager.Instance.ForwardStringToUserDevices(addMention, userIdBeingMentioneds);
        }
        public void SetSeen(long userIdBeingMentioned, long messageId)
        {
            OperationRedirectHelper.OperationRedirectedToNode(
                _UserIdToNodeId.GetNodeIdFromIdentifier(userIdBeingMentioned),
                () =>
                {
                    SetSeen_Here(userIdBeingMentioned, messageId);
                },
                () => new SetSeenMention(userIdBeingMentioned, messageId)
            );
        }
        #endregion Public
        #region Private
        private void Dispose() {
            _CancellationTokenSourceDisposed.Cancel();
        }
        #endregion Private
#endregion Methods
    }
}
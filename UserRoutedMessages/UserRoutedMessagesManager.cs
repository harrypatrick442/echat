using Core;
using Core.Exceptions;
using Core.Handlers;
using Core.Interfaces;
using Core.Threading;
using InterserverComs;
using JSON;
using Logging;
using MessageTypes.Internal;
using Nodes;
using Shutdown;
using UserRouting;

namespace UserRoutedMessages
{
    public sealed class UserRoutedMessagesManager
    {
        private static UserRoutedMessagesManager? _Instance;
        public static UserRoutedMessagesManager Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(UserRoutedMessagesManager));
            _Instance = new UserRoutedMessagesManager();
            return _Instance;
        }
        public static UserRoutedMessagesManager Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(UserRoutedMessagesManager));
                return _Instance;
            }
        }
        private RepeatExceptionLogHandler _ExceptionHandler;
        private int _MyNodeId;
        private Action _RemoveMessageTypeMappings;
        private UserRoutedMessagesManager() {
            _MyNodeId = Nodes.Nodes.Instance.MyId;
            _ExceptionHandler = new RepeatExceptionLogHandler(GlobalConstants.Delays.LOG_REPEAT_ERROR, Logs.Default);
            _RemoveMessageTypeMappings = InterserverMessageTypeMappingsHandler.Instance.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    { InterserverMessageTypes.UserRoutedMessagesMessage , HandleUserRoutedMessagesMessage}
                }
            );
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.UserRoutedMessagesManager);
        }
        public void ForwardObjectToUserDevices<TMessage>(TMessage message, params long[] userIds)
        {
            if (message == null|| userIds==null) return;
            string serializedMessage = Json.Serialize(message);
            ForwardStringToUserDevices(serializedMessage, userIds);
        }
        public void ForwardStringToUserDevices(string serializedMessage, params long[] userIds)
        {
            if (string.IsNullOrEmpty(serializedMessage)) return;
            NodeAndAssociatedUserIdsSessionIds[] nodeAndAssociatedUserIdsSessionIds_s = CoreUserRoutingTable
                .Instance.GetNodeAndAssociatedUserIdsSessionIds(userIds,
                out long[] userIdsRequiringForwardingToUsersMachines);
            ParallelOperationHelper.RunInParallelNoReturn(
                nodeAndAssociatedUserIdsSessionIds_s,
                Get_ForwardToUserDevices_SendToSpecificNode(serializedMessage),
                GlobalConstants.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_USERS_DEVICES
            );
        }
        private Action<NodeAndAssociatedUserIdsSessionIds> Get_ForwardToUserDevices_SendToSpecificNode(string serializedMessage) {
            return (nodeAndAssociatedUserIdsSessionIds) =>
            {
                int nodeId = nodeAndAssociatedUserIdsSessionIds.NodeId;
                IEnumerable<long> userIds = nodeAndAssociatedUserIdsSessionIds.UserIdSessionIdss.Select(u => u.UserId);
                if (nodeId == _MyNodeId)
                {
                    ForwardToUserDevices_Here(userIds, serializedMessage);
                    return;
                }
                INodeEndpoint nodeEndpoint = InterserverPort.Instance.GetEndpointByNodeId(nodeId);

                try
                {
                    if (nodeEndpoint == null)
                    {
                        throw new Exception($"Could not get endpoint for node {nodeId}");
                    }
                    UserRoutedMessagesMessage userRoutedMessagesMessage = new UserRoutedMessagesMessage(
                        userIds.ToArray(), serializedMessage);
                    string userRoutedMessagesMessageSerialized = Json.Serialize(userRoutedMessagesMessage);
                    try
                    {
                        nodeEndpoint.SendJSONString(userRoutedMessagesMessageSerialized);
                    }
                    catch (Exception ex2)
                    {

                        throw new Exception($"Failed to communicate with node {nodeId}", ex2);
                    }
                }
                catch (Exception ex)
                {

                    _ExceptionHandler.Log(ex);
                }
            };
        }
        private void ForwardToUserDevices_Here(IEnumerable<long> userIds, string message)
        {
            IClientEndpoint[] endpoints = CoreUserRoutingTable.Instance.GetEndpointsForUserIds(userIds,
                out long[] userIdsDidntHave);
            if (endpoints == null) return;
            ParallelOperationHelper.RunInParallelNoReturn(
                endpoints,
                (endpoint) => {
                    try
                    {
                        endpoint.SendJSONString(message);
                    }
                    catch (Exception ex) {
                        _ExceptionHandler.Log(ex);
                    }
                },
                GlobalConstants.Threading.MAX_N_THREADS_SEND_MESSAGE_TO_USERS_DEVICES
            );
        }
        private void HandleUserRoutedMessagesMessage(InterserverMessageEventArgs e) {
            UserRoutedMessagesMessage message = Json.Deserialize<UserRoutedMessagesMessage>(e.JsonString);
            ForwardToUserDevices_Here(message.UserIds, message.SerializedMessage);
        }
        private void Dispose() {
            _RemoveMessageTypeMappings();
        }
    }
}

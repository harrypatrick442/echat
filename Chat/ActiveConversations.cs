using System.Runtime.Serialization;
using Chat.Messages.Client.Messages;
using Logging;

namespace Chat
{
    [DataContract]
    public class ActiveConversations
    {
        private const int MAX_N_ENTRIES = 100;
        private long _UserId;
        private int _Count = 0;
        private Dictionary<long, Tuple<int, ConversationType>> _MapConversationIdToMapNodeIdToConversationType;
        public void Add(long userId, int nodeId, long conversationId, ConversationType conversationType)
        {
            lock (this)
            {
                if (_UserId <= 0)
                {
                    _UserId = userId;
                }
                else {
                    if (_UserId != userId) {
                        if (_MapConversationIdToMapNodeIdToConversationType != null)
                        {
                            _Remove(_UserId, _MapConversationIdToMapNodeIdToConversationType);
                            _MapConversationIdToMapNodeIdToConversationType?.Clear();
                            _Count = 0;
                        }
                    }
                    _UserId = userId;
                }
                if (_MapConversationIdToMapNodeIdToConversationType == null)
                {
                    _MapConversationIdToMapNodeIdToConversationType = new Dictionary<long, Tuple<int, ConversationType>>
                    { { conversationId, new Tuple<int, ConversationType>(nodeId, conversationType )} };
                    _Count = 1;
                    return;
                }
                if (_Count < MAX_N_ENTRIES)
                {
                    if (_MapConversationIdToMapNodeIdToConversationType.TryAdd(conversationId, new Tuple<int, ConversationType>(nodeId, conversationType)))
                        _Count++;
                    return;
                }
                /*This should only happen in the event a user messes with client. For cleanup to fail
                would require a disconnect which would dispose and dump the lot anyway. So this is safe.*/
                _Remove(_UserId, _MapConversationIdToMapNodeIdToConversationType);
                _MapConversationIdToMapNodeIdToConversationType.Clear();
                _MapConversationIdToMapNodeIdToConversationType.Add(conversationId, new Tuple<int, ConversationType>(nodeId, conversationType));
                _Count = 1;
            }
        }
        public void Remove(long[] conversationIds)
        {
            lock (this)
            {
                if (_UserId <= 0)
                {
                    Logs.Default.Error($"This shouldn't be getting called before {nameof(_UserId)} is set");
                    return;
                }
                foreach (var groupForNode in conversationIds
                    .Select(c => _MapConversationIdToMapNodeIdToConversationType
                    .TryGetValue(c, out Tuple<int, ConversationType> nodeIdAndType)
                        ? new { conversationId = c, nodeIdAndType }
                        : null)
                    .Where(o => o != null)
                    .GroupBy(o => o.nodeIdAndType.Item1).ToArray())
                {
                    int nodeId = groupForNode.First().nodeIdAndType.Item1;
                    var conversationTypeWithConversationIds = groupForNode.GroupBy(o => o.nodeIdAndType.Item2)
                        .Select(g => new ConversationTypeWithConversationIds(
                            g.First().nodeIdAndType.Item2, g.Select(o => o.conversationId).ToArray())
                        ).ToArray();
                    try
                    {
                        ChatManager.Instance.RemoveUserFromActiveConversations(
                            _UserId,
                            nodeId,
                            conversationTypeWithConversationIds
                            );
                        foreach (var c in conversationTypeWithConversationIds)
                        {
                            foreach (long conversationId in c.ConversationIds)
                            {
                                _MapConversationIdToMapNodeIdToConversationType.Remove(conversationId);
                            }
                        }
                        _Count -= conversationTypeWithConversationIds.Sum(c => c.ConversationIds.Length);
                    }
                    catch (Exception ex) {
                        Logs.Default.Error(ex);
                    }
                }
            }
        }
        public ActiveConversations() {

        }
        public void Dispose() {
            lock (this) {
                if (_MapConversationIdToMapNodeIdToConversationType == null) return;
                if (_UserId <= 0) {
                    return;
                }
                _Remove(_UserId, _MapConversationIdToMapNodeIdToConversationType);
                _MapConversationIdToMapNodeIdToConversationType = null;
                _Count = 0;
            }
        }
        private void _Remove(long userId, IEnumerable<KeyValuePair<long, Tuple<int, ConversationType>>> keyValuePairs) {
            foreach (var groupForNode in keyValuePairs.GroupBy(
                k => k.Value.Item1))
            {
                int nodeId = groupForNode.First().Value.Item1;
                var conversationTypeWithConversationIds = groupForNode.GroupBy(k => k.Value.Item2)
                    .Select(g => new ConversationTypeWithConversationIds(
                        g.First().Value.Item2, g.Select(o => o.Key).ToArray())
                    ).ToArray();
                try
                {
                    ChatManager.Instance.RemoveUserFromActiveConversations(
                        userId,
                        nodeId,
                        conversationTypeWithConversationIds
                        );
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }
        }
    }
}

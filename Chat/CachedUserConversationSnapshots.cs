using System.Runtime.Serialization;
using Chat.Messages.Client.Messages;
using Core.Collections;
using Core.DAL;

namespace Chat
{
    [DataContract]
    public class CachedUserConversationSnapshots
    {
        public long UserId { get; protected set; }
        public long IdFromInclusive { 
            get {
                if (_ConversationSnapshots == null) return -1;
                var first = _ConversationSnapshots.First;
                if(first==null) return -1;
                return first.Id;
            } 
        }
        private OrderedDictionary<long, ConversationSnapshot> _ConversationSnapshots;
        public ConversationSnapshot[] ConversationSnapshots { 
            get { return _ConversationSnapshots?.ToArray(); }
            protected set {
                    _ConversationSnapshots =
                    value == null
                    ?null
                    :new OrderedDictionary<long, ConversationSnapshot>(c=>c.ConversationId, value);
            }
        }
        public CachedUserConversationSnapshots(ConversationSnapshot[] conversationSnapshots)
        {
            if (conversationSnapshots != null)
                _ConversationSnapshots = new OrderedDictionary<long, ConversationSnapshot>(
                    v => v.ConversationId, conversationSnapshots);
        }
        public CachedUserConversationSnapshots()
        {
        }
        public ConversationSnapshot UpdateLatestMessageOrAdd(
            ClientMessage message, long[] userIdsInConversation) {
            if (message.ConversationId==0)
            {
                throw new Exception("Something went very wrong");
            }
            ConversationSnapshot conversationSnapshot;
            bool seen = message.UserId == UserId;
            if (_ConversationSnapshots == null)
            {
                conversationSnapshot = new ConversationSnapshot(message,
                    userIdsInConversation, seen);
                _ConversationSnapshots = new OrderedDictionary<long, ConversationSnapshot>(
                    v=>v.ConversationId, conversationSnapshot);
                return conversationSnapshot;
            }
            if (_ConversationSnapshots.TryGetValue((long)message.ConversationId, out conversationSnapshot))
            {
                conversationSnapshot.UpdateWithLatestMessage(message, userIdsInConversation, seen);
                return conversationSnapshot;
            }
            conversationSnapshot = new ConversationSnapshot(message, 
                userIdsInConversation, seen);
            _ConversationSnapshots.AppendOrMoveToLast(conversationSnapshot);
            int nToTake = _ConversationSnapshots.Length - ChatConstants.CONVERSATION_SNAPSHOTS_N_ENTRIES_CACHE;
            if (nToTake > 0)
            {
                _ConversationSnapshots.RemoveNFromFirst(nToTake);
            }
            return conversationSnapshot;
        }
        public void SetSeen(long conversationId, long messageId) {
            if (_ConversationSnapshots == null) return;
            if (_ConversationSnapshots.TryGetValue(conversationId, out ConversationSnapshot conversationSnapshot))
            {
                if (conversationSnapshot.Id == messageId)
                {
                    conversationSnapshot.Seen = true;
                }
            }
        }
    }
}

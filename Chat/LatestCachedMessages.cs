using Chat.Messages.Client;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client.Requests;
using Chat.Messages.Client.Responses;
using Core.Collections;
using Core.DAL;
using JSON;
using System.Text;

namespace Chat
{
    public class LatestCachedMessages
    {
        protected class CachedClientMessage { 
            public ClientMessage Message { get; }
            private Dictionary<long, List<MessageReaction>> _MapUserIdToReactions;
            public IEnumerable<MessageReaction> Reactions { get {
                    return _MapUserIdToReactions?.Values.SelectMany(r=>r);
                } }
            private MessageUserMultimediaItem[] _UserMultimediaItems;
            public MessageUserMultimediaItem[] UserMultimediaItems { get {
                    return _UserMultimediaItems;
                } }
            public string JsonString {
                get;
                private set;
            }
            private string _CachedCommaSeperatedReactionsString;
            public string CommaSeperatedReactionsString
            {
                get
                {
                    if (_CachedCommaSeperatedReactionsString != null)
                        return _CachedCommaSeperatedReactionsString;
                    if (_MapUserIdToReactions == null || !_MapUserIdToReactions.Any())
                        return null;
                    _CachedCommaSeperatedReactionsString = string.Join(',', _MapUserIdToReactions.Values.SelectMany(v => v).Select(v => Json.Serialize(v)));
                    return _CachedCommaSeperatedReactionsString;
                }
            }
            private string _CachedCommaSeperatedUserMultimediaItemsString;
            public string CommaSeperatedUserMultimediaItemsString
            {
                get
                {
                    if (_CachedCommaSeperatedUserMultimediaItemsString != null)
                        return _CachedCommaSeperatedUserMultimediaItemsString;
                    if (_UserMultimediaItems== null || !_UserMultimediaItems.Any())
                        return null;
                    _CachedCommaSeperatedUserMultimediaItemsString = string.Join(',', _UserMultimediaItems.Select(v => Json.Serialize(v)));
                    return _CachedCommaSeperatedUserMultimediaItemsString;
                }
            }
            public CachedClientMessage(ClientMessage message, MessageReaction[] reactions, 
                MessageUserMultimediaItem[] userMultimediaItems, string jsonString) {
                Message = message;
                _MapUserIdToReactions = reactions?.GroupBy(r => r.UserId).ToDictionary(g => g.First().UserId, g => g.ToList());
                _UserMultimediaItems = userMultimediaItems;
                JsonString = jsonString;
            }
            public void Modify(ClientMessage newMessage, out bool modifiedMultimediaItems) {
                if (newMessage == null)
                    throw new ArgumentException(nameof(newMessage));
                Message.Content = newMessage.Content;
                Message.Version = newMessage.Version;
                bool hasOldUserMultimediaItems = _UserMultimediaItems != null && _UserMultimediaItems.Any();
                bool hasNewUserMultimediaItems = newMessage.UserMultimediaItems != null && newMessage.UserMultimediaItems.Any();
                if (hasOldUserMultimediaItems != hasNewUserMultimediaItems
                    ||
                    (hasNewUserMultimediaItems
                        && newMessage.UserMultimediaItems
                            .Where(n => _UserMultimediaItems
                                .Where(m => m.MultimediaToken != n.MultimediaToken
                            )
                            .Any()
                         ).Any()
                    ))
                {
                    _UserMultimediaItems = hasNewUserMultimediaItems
                        ?newMessage.UserMultimediaItems
                            .Select(m => new MessageUserMultimediaItem(Message.Id, m.MultimediaToken, m.Status, m.XRating, m.Description))
                            .ToArray()
                        :null;
                    _CachedCommaSeperatedUserMultimediaItemsString = null;
                    modifiedMultimediaItems = true;
                }
                else
                {
                    modifiedMultimediaItems = false;
                }
                Message.UserMultimediaItems = null;
                JsonString = Json.Serialize(Message);
            }
            public bool RemoveReaction(MessageReaction reaction) {
                if (_MapUserIdToReactions == null)
                    return false;
                if (!_MapUserIdToReactions.TryGetValue(reaction.UserId, out List<MessageReaction> reactions))
                    return false;
                MessageReaction toRemove = reactions.Where(r => r.Code == reaction.Code 
                    && r.MultimediaToken == r.MultimediaToken).FirstOrDefault();
                if (toRemove == null) 
                    return false;
                _CachedCommaSeperatedReactionsString = null;
                reactions.Remove(toRemove);
                if (reactions.Count < 1)
                    _MapUserIdToReactions.Remove(reaction.UserId);
                return true;
            }
            public bool AddReaction(MessageReaction reaction)
            {
                if (reaction == null) return false;
                _CachedCommaSeperatedReactionsString = null;
                if (_MapUserIdToReactions == null)
                {
                    _MapUserIdToReactions = new Dictionary<long, List<MessageReaction>> {
                        { reaction.UserId, new List<MessageReaction>{ reaction }}
                    };
                    return true;
                }
                if(!_MapUserIdToReactions.TryGetValue(reaction.UserId, out List<MessageReaction> reactions))
                {
                    reactions = new List<MessageReaction> { reaction };
                    _MapUserIdToReactions.Add(reaction.UserId, reactions);
                    return true;
                }
                if (reactions.Where(r => r.Code == reaction.Code
                    && r.MultimediaToken == r.MultimediaToken).Any())
                    return false;
                reactions.Add(reaction);
                return true;
            }
        }
        private readonly int _MaxNMessages;
        protected string _CachedJSONArrayString = null,
            _CachedReactionsJsonArrayString = null,
            _CachedUserMultimediaItemsJsonArrayString = null;

        private OrderedDictionary<long, CachedClientMessage> _CachedClientMessagesBuffer;
        private Action<ClientMessage[], MessageReaction[], MessageUserMultimediaItem[]> _FlushMessages;
        private bool _Changed = false;
        public LatestCachedMessages(int maxNMessages, 
            Action<ClientMessage[], MessageReaction[], MessageUserMultimediaItem[]> flushMessages) {
            _MaxNMessages = maxNMessages;
            _FlushMessages = flushMessages;
        }
        public void Initialize(ClientMessage[] messages, MessageReaction[] reactions, MessageUserMultimediaItem[] messageUserMultimediaItems) {
            if (messages == null)
            {
                _CachedClientMessagesBuffer = new OrderedDictionary<long, CachedClientMessage>(c=>c.Message.Id);
                return;
            }
            Func<long, MessageReaction[]> getReactionsForMessage;
            if (reactions != null)
            {
                Dictionary<long, MessageReaction[]> mapMessageIdToReactions = reactions?
                    .GroupBy(r => r.MessageId)
                    .ToDictionary(g => g.First().MessageId, g => g.ToArray());
                getReactionsForMessage
                 = (messageId) =>
                 {
                     mapMessageIdToReactions.TryGetValue(messageId, out MessageReaction[] reactions);
                     return reactions;
                 };
            }
            else {
                getReactionsForMessage = (messageId) => null;
            }
            Func<long, MessageUserMultimediaItem[]> getUserMultimediaItemsForMessage;
            if (messageUserMultimediaItems != null) {
                Dictionary<long, MessageUserMultimediaItem[]> mapMessageIdToUserMultimediaItems = messageUserMultimediaItems?
                    .GroupBy(r => r.MessageId)
                    .ToDictionary(g => g.First().MessageId, g => g.ToArray());
                getUserMultimediaItemsForMessage = (messageId) =>
                {
                    mapMessageIdToUserMultimediaItems.TryGetValue(messageId, out MessageUserMultimediaItem[] items);
                    return items;
                };
            }
            else {
                getUserMultimediaItemsForMessage = (messageId) =>null;
            }

            CachedClientMessage[] nonNullMessages = messages
                .Where(m => m != null)
                .Select(m=>new CachedClientMessage(m, getReactionsForMessage(m.Id), getUserMultimediaItemsForMessage(m.Id), Json.Serialize(m)))
                .ToArray();
            _CachedClientMessagesBuffer = new OrderedDictionary<long, CachedClientMessage>(c => c.Message.Id, nonNullMessages);
        }
        public void Add(ClientMessage message, string jsonString)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (jsonString==null)
            {
                jsonString = Json.Serialize(message);
            }
            lock (_CachedClientMessagesBuffer)
            {
                _CachedClientMessagesBuffer.AppendOrMoveToLast(new CachedClientMessage(message, null, message.UserMultimediaItems?.Select(m=>new MessageUserMultimediaItem(message.Id, m.MultimediaToken, m.Status, m.XRating, m.Description)).ToArray(), jsonString));
                _Changed = true;
                _CachedJSONArrayString = null;
                int nToTake = _CachedClientMessagesBuffer.Length - _MaxNMessages;
                if (nToTake>0)
                {
                    _CachedClientMessagesBuffer.TakeFromFirstWhile((i) => nToTake-- > 0);
                }
            }
        }
        public long[] Delete(long userId, long[] messageIds, bool canDeleteAnyMessage) {
            List<long> deletedIds = new List<long>();
            lock (_CachedClientMessagesBuffer) {
                foreach (long messageId in messageIds) {
                    if (canDeleteAnyMessage) {
                        if (_CachedClientMessagesBuffer.RemoveByKey(messageId))
                        {
                            _CachedJSONArrayString = null;
                            _Changed = true;
                            deletedIds.Add(messageId);
                        }
                        continue;
                    }
                    if (!_CachedClientMessagesBuffer.TryGetValue(messageId, out CachedClientMessage cachedClientMessage))
                        continue;
                    if (cachedClientMessage.Message.UserId != userId)
                        continue;
                    _CachedClientMessagesBuffer.RemoveByKey(messageId);
                    _CachedJSONArrayString = null;
                    _Changed = true;
                    deletedIds.Add(messageId);
                }
            }
            return deletedIds.ToArray();
        }
        public bool Modify(ClientMessage message)
        {
            lock (_CachedClientMessagesBuffer)
            {
                if (!_CachedClientMessagesBuffer.TryGetValue(message.Id, out CachedClientMessage cachedClientMessage))
                    return false;
                if (cachedClientMessage.Message.UserId != message.UserId)
                    return false;
                cachedClientMessage.Modify(message, out bool modifiedMultimediaItems);
                if (modifiedMultimediaItems) {
                    _CachedUserMultimediaItemsJsonArrayString = null;
                }
                _CachedJSONArrayString = null;
                _Changed = true;
            }
            return true;
        }
        public void ReactToMessage(MessageReaction reaction)
        {
            lock (_CachedClientMessagesBuffer)
            {
                if (!_CachedClientMessagesBuffer.TryGetValue(reaction.MessageId, out CachedClientMessage cachedClientMessage))
                    return;
                if (cachedClientMessage.Message.UserId != reaction.UserId)
                    return;
                if (cachedClientMessage.AddReaction(reaction))
                {
                    _CachedReactionsJsonArrayString = null;
                    _Changed = true;
                }
            }
        }
        public void UnreactToMessage(MessageReaction reaction)
        {
            lock (_CachedClientMessagesBuffer)
            {
                if (!_CachedClientMessagesBuffer.TryGetValue(reaction.MessageId, out CachedClientMessage cachedClientMessage))
                    return;
                if (cachedClientMessage.Message.UserId != reaction.UserId)
                    return;
                if (cachedClientMessage.RemoveReaction(reaction))
                {
                    _CachedReactionsJsonArrayString = null;
                    _Changed = true;
                }
            }
        }
        public void FlushIfChanged() {
            ClientMessage[] messages;
            MessageReaction[] reactions;
            MessageUserMultimediaItem[] userMultimediaItems;
            lock (_CachedClientMessagesBuffer)
            {
                if (!_Changed) return;
                _Changed = false;
                var cachedClientMessages = _CachedClientMessagesBuffer.ToArray();
                messages = cachedClientMessages.Select(c=>c.Message).ToArray();
                reactions = cachedClientMessages.Select(c => c.Reactions)
                    .Where(r => r != null ).SelectMany(r => r).ToArray();
                userMultimediaItems = cachedClientMessages.Select(c => c.UserMultimediaItems)
                    .Where(r => r != null).SelectMany(r => r).ToArray();
            }
            _FlushMessages?.Invoke(messages, reactions, userMultimediaItems);
        }
        public string GetJSONArrayString(out string reactionsJsonArrayString, out string userMultimediaItemsJsonArrayString)
        {
            reactionsJsonArrayString = null;
            lock (_CachedClientMessagesBuffer)
            {
                CachedClientMessage[] cachedClientMessages = null;
                if (_CachedJSONArrayString == null)
                {
                    StringBuilder sbMessages = new StringBuilder($"[");
                    cachedClientMessages = _CachedClientMessagesBuffer.ToArray();
                    bool first = true; ;
                    foreach (CachedClientMessage cachedClientMessage in cachedClientMessages)
                    {
                        if (first) first = false;
                        else sbMessages.Append(',');
                        sbMessages.Append(cachedClientMessage.JsonString);
                    }
                    sbMessages.Append(']');
                    _CachedJSONArrayString = sbMessages.ToString();
                }
                if (_CachedReactionsJsonArrayString == null)
                {
                    StringBuilder sbReactions = new StringBuilder($"[");
                    bool firstReactions = true;
                    foreach (CachedClientMessage cachedClientMessage in cachedClientMessages?? _CachedClientMessagesBuffer.ToArray())
                    {
                        string reactionsString = cachedClientMessage.CommaSeperatedReactionsString;
                        if (reactionsString != null)
                        {
                            if (firstReactions) firstReactions = false;
                            else sbReactions.Append(',');
                            sbReactions.Append(reactionsString);
                        }
                    }
                    if (!firstReactions)
                    {
                        sbReactions.Append(']');
                        _CachedReactionsJsonArrayString = sbReactions.ToString();
                    }
                }






                if (_CachedUserMultimediaItemsJsonArrayString == null)
                {
                    StringBuilder sbUserMultimediaItems = new StringBuilder($"[");
                    bool first = true;
                    foreach (CachedClientMessage cachedClientMessage in cachedClientMessages ?? _CachedClientMessagesBuffer.ToArray())
                    {
                        string userMultimediaItemsString = cachedClientMessage.CommaSeperatedUserMultimediaItemsString;
                        if (userMultimediaItemsString != null)
                        {
                            if (first) first = false;
                            else sbUserMultimediaItems.Append(',');
                            sbUserMultimediaItems.Append(userMultimediaItemsString);
                        }
                    }
                    if (!first)
                    {
                        sbUserMultimediaItems.Append(']');
                        _CachedUserMultimediaItemsJsonArrayString = sbUserMultimediaItems.ToString();
                    }
                }
                userMultimediaItemsJsonArrayString = _CachedUserMultimediaItemsJsonArrayString;
                reactionsJsonArrayString = _CachedReactionsJsonArrayString;
                return _CachedJSONArrayString;
            }
        }
        public void GetNMessagesFromEnd(int? nEntries, out ClientMessage[] messages,
            out MessageReaction[] reactions, out MessageUserMultimediaItem[] userMultimediaItems)
        {
            lock (_CachedClientMessagesBuffer)
            {
                var cachedClientMessages = nEntries==null
                    ?_CachedClientMessagesBuffer.ToArray()
                    :_CachedClientMessagesBuffer.GetNEntriesFromEnd((int)nEntries);
                if (cachedClientMessages == null)
                {
                    reactions = null;
                    userMultimediaItems = null;
                    messages = null;
                    return;
                }
                List<ClientMessage> messagesInternal = new List<ClientMessage>();
                List<MessageReaction> reactionsInternal= null;
                List<MessageUserMultimediaItem> userMultimediaItemsInternal = null;
                foreach (CachedClientMessage cachedClientMessage in cachedClientMessages)
                {
                    messagesInternal.Add(cachedClientMessage.Message);
                    var r = cachedClientMessage.Reactions;
                    if (r != null)
                    {
                        if (reactionsInternal == null) reactionsInternal = new List<MessageReaction>();
                        reactionsInternal.AddRange(r);
                    }
                    var u = cachedClientMessage.UserMultimediaItems;
                    if (u != null)
                    {
                        if (userMultimediaItemsInternal== null) userMultimediaItemsInternal = new List<MessageUserMultimediaItem>();
                        userMultimediaItemsInternal.AddRange(u);
                    }
                }
                messages = messagesInternal.ToArray();
                reactions = reactionsInternal?.ToArray();
                userMultimediaItems = userMultimediaItemsInternal?.ToArray();
            }
            /*
            messages = GetCachedRange(idFromInclusive, idToExclusive, nEntries,
                out long minIdInclusive, out long maxIdInclusive, out reactions);
            int nMoreMessagesToTake = nEntries - messages.Length;
            if (nMoreMessagesToTake < 0)
                return;
            if (idFromInclusive!=null&&minIdInclusive <= (long)idFromInclusive) return;
            ClientMessage[] otherMessages = DalMessagesSQLite.Instance.ReadRange(_ConversationId, nMoreMessagesToTake, idFromInclusive, minIdInclusive, out MessageReaction[] otherReactions);
            if (otherMessages == null) return;
            if (messages == null)
            {
                messages = otherMessages;
                reactions = otherReactions;
                return;
            };
            messages = otherMessages.Concat(messages).ToArray();
            if (otherReactions == null) return;
            if (reactions == null)
            {
                reactions = otherReactions;
                return;
            }
            reactions = otherReactions.Concat(reactions).ToArray();*/

        }
        private int ConstrainNEntries(int? nEntries)
        {

            if (nEntries == null || (int)nEntries <= 0)
            {
                   return Configurations.Lengths.N_MESSAGES_LOAD_PUBLIC_CHATROOM;
            }
            if ((int)nEntries > Configurations.Lengths.N_MESSAGES_LOAD_PUBLIC_CHATROOM)
                return Configurations.Lengths.N_MESSAGES_LOAD_PUBLIC_CHATROOM;
            return (int)nEntries;
        }
    }
}
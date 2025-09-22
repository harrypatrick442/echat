using DataMemberNames.Client.Chat.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat
{
    [DataContract]
    public class Conversationf
    {
        private HashSet<long> _UserIds;
        [JsonPropertyName(ConversationDataMemberNames.UserIds)]
        [JsonInclude]
        [DataMember(Name = ConversationDataMemberNames.UserIds)]
        public long[] UserIds { get { return _UserIds?.ToArray(); } protected set { _UserIds = value == null ? new HashSet<long>() : new HashSet<long>(value); } }
        [JsonPropertyName(ConversationDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ConversationDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(ConversationDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = ConversationDataMemberNames.ConversationType)]
        public ConversationType ConversationType {
            get;
            protected set; 
        }
        [JsonPropertyName(ConversationDataMemberNames.ShortName)]
        [JsonInclude]
        [DataMember(Name = ConversationDataMemberNames.ShortName)]
        public string ShortName
        {
            get;
            protected set;
        }
        [JsonPropertyName(ConversationDataMemberNames.IsNonVolatile)]
        [JsonInclude]
        [DataMember(Name = ConversationDataMemberNames.IsNonVolatile)]
        public bool IsNonVolatile
        {
            get;
            protected set;
        }
        public Conversation(long conversationId, ConversationType conversationType, 
            long[] userIds, string shortName, bool isNonVolatile) 
            : this(conversationId,
             conversationType, userIds, isNonVolatile)
        {
            ShortName = shortName;
        }
        public Conversation(long conversationId,
            ConversationType conversationType, long[] userIds, bool isNonVolatile)
        {
            ConversationId = conversationId;
            ConversationType = conversationType;
            _UserIds = userIds==null?null:new HashSet<long>(userIds);
           IsNonVolatile = isNonVolatile;
        }
        protected Conversation() { }
        public long[] GetUserIdsInConversationForSnapshot() {
            switch (ConversationType)
            {
                case ConversationType.Pm:
                case ConversationType.Wall:
                    return UserIds;

                case ConversationType.PublicChatroom:
                    throw new Exception("Shouldnt be taking this route");
                case ConversationType.GroupChat:
                default:
                    return _UserIds?.Take(4).ToArray();
            }
        }
        public bool ContainsUser(long userId) {
            return _UserIds!=null?_UserIds.Contains(userId):false;
        }
        public void EnsureHasUserIds(params long[] userIds)
        {
            lock (this)
            {
                if (_UserIds == null) _UserIds = new HashSet<long>(userIds);
                else
                {
                    foreach (long userId in userIds)
                    {
                        if (!_UserIds.Contains(userId))
                            _UserIds.Add(userId);
                    }
                }
            }
        }
    }
}

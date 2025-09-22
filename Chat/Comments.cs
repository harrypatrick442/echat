using Chat.DataMemberNames.Messages;
using Chat.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;

namespace Chat
{
    [DataContract]
    public class Comments:IActiveUsers
    {
        [JsonIgnore]
        private HashSet<long> _ActiveUsers = new HashSet<long>();
        [JsonPropertyName(CommentsDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = CommentsDataMemberNames.ConversationId)]
        public long ConversationId { get; set; }
        [JsonPropertyName(CommentsDataMemberNames.OwnerUserId)]
        [JsonInclude]
        [DataMember(Name = CommentsDataMemberNames.OwnerUserId)]
        public long OwnerUserId { get; set; }
        [JsonPropertyName(CommentsDataMemberNames.CommentsScopeType)]
        [JsonInclude]
        [DataMember(Name = CommentsDataMemberNames.CommentsScopeType)]
        public CommentsScopeType ScopeType { get; set; }
        [JsonPropertyName(CommentsDataMemberNames.ScopeId)]
        [JsonInclude]
        [DataMember(Name = CommentsDataMemberNames.ScopeId)]
        public long ScopeId { get; set; }
        [JsonPropertyName(CommentsDataMemberNames.Count)]
        [JsonInclude]
        [DataMember(Name = CommentsDataMemberNames.Count)]
        public int Count { get; protected set; }

        public Comments(long conversationId, CommentsScopeType scopeType,
            long ownerUserId, long scopeId) {
            ConversationId = conversationId;
            ScopeType = scopeType;
            OwnerUserId = ownerUserId;
            ScopeId = scopeId;
        }
        public void IncrementCount()
        {
            Count++;
        }
        public void DecrementCount()
        {
            Count--;
        }
        protected Comments() { }
        public long[] ActiveUsersAsArray()
        {
            lock (_ActiveUsers)
            {
                return _ActiveUsers.ToArray();
            }
        }
        public void AddActiveUser(long userId) {
            lock (_ActiveUsers)
            {
                _ActiveUsers.Add(userId);
            }
        }
        public void RemoveActiveUser(long userId)
        {
            lock (_ActiveUsers)
            {
                _ActiveUsers.Remove(userId);
            }
        }

        public void UsingUserIds(Action<IEnumerable<long>> callback)
        {
            lock (_ActiveUsers)
            {
                callback(_ActiveUsers);
            }
        }

        public long[] UserIdsToArray()
        {
            lock (_ActiveUsers)
            {
                return _ActiveUsers.ToArray();
            }
        }
    }
}

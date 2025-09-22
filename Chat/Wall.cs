using Chat.DataMemberNames.Messages;
using Chat.Interfaces;
using Core.Chat;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;

namespace Chat
{
    [DataContract]
    public class Wall:IActiveUsers
    {
        private HashSet<long>
            _ActiveUsers = new HashSet<long>();
        [JsonPropertyName(WallDataMemberNames.VisibleTo)]
        [JsonInclude]
        [DataMember(Name = WallDataMemberNames.VisibleTo)]
        public VisibleTo VisibleTo{ get; protected set; }
        [JsonPropertyName(WallDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = WallDataMemberNames.ConversationId)]
        public long ConversationId { get; set; }
        [JsonPropertyName(WallDataMemberNames.OwnerUserId)]
        [JsonInclude]
        [DataMember(Name = WallDataMemberNames.OwnerUserId)]
        public long OwnerUserId { get; set; }
        public Wall(long conversationId, VisibleTo visibleTo, long ownerUserId) {
            ConversationId = conversationId;
            VisibleTo = visibleTo;
            OwnerUserId = ownerUserId;
            _ActiveUsers = new HashSet<long>();
        }
        protected Wall(){

        }

        public long[] ActiveUsersAsArray()
        {
            lock (_ActiveUsers)
                return _ActiveUsers.ToArray();
            {
            }
        }
        public void AddActiveUser(long userId)
        {
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

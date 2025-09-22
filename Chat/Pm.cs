using Chat.DataMemberNames.Messages;
using Core.Chat;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat
{
    [DataContract]
    public class Pm:IConversation
    {
        private long _LowestUserId;
        [JsonPropertyName(PmDataMemberNames.LowestUserId)]
        [JsonInclude]
        [DataMember(Name = PmDataMemberNames.LowestUserId)]
        public long LowestUserId { get { return _LowestUserId; } protected set { _LowestUserId = value; } }
        private long _HighestUserId;
        [JsonPropertyName(PmDataMemberNames.HighestUserId)]
        [JsonInclude]
        [DataMember(Name = PmDataMemberNames.HighestUserId)]
        public long HighestUserId { get { return _HighestUserId; } protected set { _HighestUserId = value; } }
        private long _ConversationId;
        [JsonPropertyName(PmDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name =PmDataMemberNames.ConversationId)]
        public long ConversationId { get { return _ConversationId; } set { _ConversationId = value; } }
        public Pm(long userIdA, long userIdB, long conversationId) {
            if (userIdA > userIdB)
            {
                _LowestUserId = userIdB;
                _HighestUserId = userIdA;
            }
            else {
                _LowestUserId = userIdA;
                _HighestUserId = userIdB;
            }
            _ConversationId = conversationId;
        }
        protected Pm() { }

        public void UsingUserIds(Action<IEnumerable<long>> callback)
        {
            callback(new long[] { LowestUserId, HighestUserId });
        }

        public long[] UserIdsToArray()
        {
            return new long[] { LowestUserId, HighestUserId };
        }
    }
}

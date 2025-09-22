using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nodes
{
    [DataContract]
    public class UserIdSessionIds
    {
        private long _UserId;
        [JsonPropertyName(UserIdSessionIdsDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserIdSessionIdsDataMemberNames.UserId, EmitDefaultValue = true)]
        public long UserId { get { return _UserId; } protected set { _UserId = value; } }
        private long[] _SessionIds;
        [JsonPropertyName(UserIdSessionIdsDataMemberNames.SessionIds)]
        [JsonInclude]
        [DataMember(Name = UserIdSessionIdsDataMemberNames.SessionIds, EmitDefaultValue = true)]
        public long[] SessionIds{ get { return _SessionIds; } protected set { _SessionIds = value; } }
        public UserIdSessionIds(long userId, long[] sessionIds) {
            _UserId = userId;
            _SessionIds = sessionIds;
        }
        protected UserIdSessionIds() { }

    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Messages;
using Core.DataMemberNames;
using UsersEnums;

namespace Users
{
    [DataContract]
    public class AssociateRequest
    {
        private long _RequestUniqueIdentifier;
        [JsonPropertyName(AssociateRquestDataMemberNames.RequestUniqueIdentifier)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestDataMemberNames.RequestUniqueIdentifier)]
        public long RequestUniqueIdentifier { get { return _RequestUniqueIdentifier; } protected set { _RequestUniqueIdentifier = value; } }
        private long _UserId;
        [JsonPropertyName(AssociateRquestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestDataMemberNames.UserId)]
        public long UserId { get { return _UserId; } protected set { _UserId = value; } }
        private AssociateType _AssociateType;
        [JsonPropertyName(AssociateRquestDataMemberNames.AssociateType)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestDataMemberNames.AssociateType)]
        public AssociateType AssociateType { get { return _AssociateType; } protected set { _AssociateType = value; } }
        private long _SentAtUTCMilliseconds;
        [JsonPropertyName(AssociateRquestDataMemberNames.SentAtUTCMilliseconds)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestDataMemberNames.SentAtUTCMilliseconds)]
        public long SentAtUTCMilliseconds { get { return _SentAtUTCMilliseconds; } protected set { _SentAtUTCMilliseconds = value; } }
        private bool _IsCounterRequest;
        [JsonPropertyName(AssociateRquestDataMemberNames.IsCounterRequest)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestDataMemberNames.IsCounterRequest)]
        public bool IsCounterRequest { get { return _IsCounterRequest; } protected set { _IsCounterRequest = value; } }
        public AssociateRequest(long userId, long requestUniqueIdentifier, AssociateType associateType, 
            long sentAtUTCMilliseconds, bool isCounterRequest) {

            _UserId = userId;
            _RequestUniqueIdentifier = requestUniqueIdentifier;
            _AssociateType = associateType;
            _SentAtUTCMilliseconds = sentAtUTCMilliseconds;
            _IsCounterRequest = isCounterRequest;
        }
        protected AssociateRequest() { }

    }
}

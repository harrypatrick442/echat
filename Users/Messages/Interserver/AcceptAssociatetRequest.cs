using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;
using UsersEnums;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class AcceptAssociatetRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(AcceptRequestRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = AcceptRequestRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(AcceptRequestRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = AcceptRequestRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        private long _RequestUniqueIdentifier;
        [JsonPropertyName(AcceptRequestRequestDataMemberNames.RequestUniqueIdentifier)]
        [JsonInclude]
        [DataMember(Name = AcceptRequestRequestDataMemberNames.RequestUniqueIdentifier)]
        public long RequestUniqueIdentifier
        {
            get { return _RequestUniqueIdentifier; }
            protected set { _RequestUniqueIdentifier = value; }
        }
        private AssociateType? _LimitTo;
        [JsonPropertyName(AcceptRequestRequestDataMemberNames.LimitTo)]
        [JsonInclude]
        [DataMember(Name = AcceptRequestRequestDataMemberNames.LimitTo)]
        public AssociateType? LimitTo
        {
            get { return _LimitTo; }
            protected set { _LimitTo = value; }
        }
        public AcceptAssociatetRequest(long myUserId, long otherUserId, long requestUniqueIdentifier, AssociateType? limitTo)
            : base(InterserverMessageTypes.UsersAcceptRequest)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
            _RequestUniqueIdentifier = requestUniqueIdentifier;
            _LimitTo = limitTo;
        }
        protected AcceptAssociatetRequest()
            : base(InterserverMessageTypes.UsersAcceptRequest) { }
    }
}

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
    public class CounterRequestRequest : TicketedMessageBase
    {
        private AssociateType _CounterAssociateType;
        [JsonPropertyName(CounterRequestRequestDataMemberNames.CounterAssociateType)]
        [JsonInclude]
        [DataMember(Name = CounterRequestRequestDataMemberNames.CounterAssociateType)]
        public AssociateType CounterAssociateType
        {
            get { return _CounterAssociateType; }
            protected set { _CounterAssociateType = value; }
        }
        private long _MyUserId;
        [JsonPropertyName(CounterRequestRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = CounterRequestRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(CounterRequestRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = CounterRequestRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        public CounterRequestRequest(long myUserId, long otherUserId, AssociateType associationTypesToKeep)
            : base(InterserverMessageTypes.UsersCounterRequest)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
            _CounterAssociateType = associationTypesToKeep;
        }
        protected CounterRequestRequest()
            : base(InterserverMessageTypes.UsersCounterRequest) { }
    }
}

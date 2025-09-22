using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class RejectRequestRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(RejectRequestRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = RejectRequestRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(RejectRequestRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = RejectRequestRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        public RejectRequestRequest(long myUserId, long otherUserId)
            : base(InterserverMessageTypes.UsersRejectRequest)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
        }
        protected RejectRequestRequest()
            : base(InterserverMessageTypes.UsersRejectRequest) { }
    }
}

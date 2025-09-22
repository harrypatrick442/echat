using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class CancelSentRequestRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(CancelSentRequestRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = CancelSentRequestRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(CancelSentRequestRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = CancelSentRequestRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        public CancelSentRequestRequest(long myUserId, long otherUserId)
            : base(InterserverMessageTypes.UsersCancelSentRequest)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
        }
        protected CancelSentRequestRequest()
            : base(InterserverMessageTypes.UsersCancelSentRequest) { }
    }
}

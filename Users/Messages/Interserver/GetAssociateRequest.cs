using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetAssociateRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(GetAssociateRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetAssociateRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(GetAssociateRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = GetAssociateRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        public GetAssociateRequest(long myUserId, long otherUserId)
            : base(InterserverMessageTypes.UsersGetAssociate)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
        }
        protected GetAssociateRequest()
            : base(InterserverMessageTypes.UsersGetAssociate) { }
    }
}

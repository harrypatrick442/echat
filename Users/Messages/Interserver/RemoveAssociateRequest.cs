using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class RemoveAssociateRequest : TicketedMessageBase
    {
        private long _MyUserId;
        [JsonPropertyName(RemoveAssociateRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = RemoveAssociateRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(RemoveAssociateRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = RemoveAssociateRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        public RemoveAssociateRequest(long myUserId, long otherUserId)
            : base(InterserverMessageTypes.UsersRemoveAssociate)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
        }
        protected RemoveAssociateRequest()
            : base(InterserverMessageTypes.UsersRemoveAssociate) { }
    }
}

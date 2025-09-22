using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetAssociateReceivedRequestRequest : TicketedMessageBase
    {
        private long _OnAssociateUserId;
        [JsonPropertyName(GetAssociateReceivedRequestRequestDataMemberNames.OnAssociateUserId)]
        [JsonInclude]
        [DataMember(Name = GetAssociateReceivedRequestRequestDataMemberNames.OnAssociateUserId)]
        public long OnAssociateUserId
        {
            get { return _OnAssociateUserId; }
            protected set { _OnAssociateUserId = value; }
        }
        private long _FromAssociateUserId;
        [JsonPropertyName(GetAssociateReceivedRequestRequestDataMemberNames.FromAssociateUserId)]
        [JsonInclude]
        [DataMember(Name = GetAssociateReceivedRequestRequestDataMemberNames.FromAssociateUserId)]
        public long FromAssociateUserId
        {
            get { return _FromAssociateUserId; }
            protected set { _FromAssociateUserId = value; }
        }
        public GetAssociateReceivedRequestRequest(long onAssociateUserId, long fromAssociateUserId)
            : base(InterserverMessageTypes.UsersGetAssociate)
        {
            _OnAssociateUserId = onAssociateUserId;
            _FromAssociateUserId = fromAssociateUserId;
        }
        protected GetAssociateReceivedRequestRequest()
            : base(InterserverMessageTypes.UsersGetAssociate) { }
    }
}

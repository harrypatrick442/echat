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
    public class RequestAssociateRequest : TicketedMessageBase
    {
        [JsonPropertyName(RequestAssociateRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = RequestAssociateRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(RequestAssociateRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = RequestAssociateRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(RequestAssociateRequestDataMemberNames.AssociateType)]
        [JsonInclude]
        [DataMember(Name = RequestAssociateRequestDataMemberNames.AssociateType)]
        public AssociateType AssociateType
        {
            get;
            protected set;
        }

        public RequestAssociateRequest(long myUserId, long otherUserId, AssociateType associateType)
            : base(InterserverMessageTypes.UsersRequestAssociate)
        {
            MyUserId = myUserId;
            OtherUserId = otherUserId;
            AssociateType = associateType;
        }
        protected RequestAssociateRequest()
            : base(InterserverMessageTypes.UsersRequestAssociate) { }
    }
}

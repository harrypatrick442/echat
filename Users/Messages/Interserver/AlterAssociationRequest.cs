using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using UsersEnums;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class AlterAssociationRequest : TicketedMessageBase
    {
        [JsonPropertyName(AlterAssociationRequestDataMemberNames.AssociateType)]
        [JsonInclude]
        [DataMember(Name = AlterAssociationRequestDataMemberNames.AssociateType)]
        public AssociateType AssociateType
        {
            get;
            protected set;
        }
        [JsonPropertyName(AlterAssociationRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = AlterAssociationRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(AlterAssociationRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = AlterAssociationRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get;
            protected set;
        }
        public AlterAssociationRequest(long myUserId, long otherUserId, AssociateType associationTypes)
            : base(MessageTypes.UsersAlterAssociate)
        {
            MyUserId = myUserId;
            OtherUserId = otherUserId;
            AssociateType = associationTypes;
        }
        protected AlterAssociationRequest()
            : base(MessageTypes.UsersAlterAssociate) { }
    }
}

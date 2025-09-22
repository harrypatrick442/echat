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
    public class DowngradeAssociateRequest : TicketedMessageBase
    {
        private AssociateType _AssociationTypesToKeep;
        [JsonPropertyName(DowngradeAssociateRequestDataMemberNames.AssociationTypesToKeep)]
        [JsonInclude]
        [DataMember(Name = DowngradeAssociateRequestDataMemberNames.AssociationTypesToKeep)]
        public AssociateType AssociationTypesToKeep
        {
            get { return _AssociationTypesToKeep; }
            protected set { _AssociationTypesToKeep = value; }
        }
        private long _MyUserId;
        [JsonPropertyName(DowngradeAssociateRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = DowngradeAssociateRequestDataMemberNames.MyUserId)]
        public long MyUserId
        {
            get { return _MyUserId; }
            protected set { _MyUserId = value; }
        }
        private long _OtherUserId;
        [JsonPropertyName(DowngradeAssociateRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = DowngradeAssociateRequestDataMemberNames.OtherUserId)]
        public long OtherUserId
        {
            get { return _OtherUserId; }
            protected set { _OtherUserId = value; }
        }
        protected DowngradeAssociateRequest(string type, long myUserId, long otherUserId,
            AssociateType associationTypesToKeep)
            : base(type)
        {
            _MyUserId = myUserId;
            _OtherUserId = otherUserId;
            _AssociationTypesToKeep = associationTypesToKeep;
        }
        protected DowngradeAssociateRequest()
            : base(null) { }
        public static DowngradeAssociateRequest Interserver(long myUserId, long otherUserId, AssociateType associateTypesToKeep) {
            return new DowngradeAssociateRequest(InterserverMessageTypes.UsersDowngradeAssociate, myUserId, otherUserId, associateTypesToKeep);
        }
    }
}

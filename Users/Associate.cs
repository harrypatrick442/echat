using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.Interfaces;
using UsersEnums;

namespace Users
{
    [DataContract]
    public class Associate:IAssociateUserId
    {
        private long _UserId;
        [JsonPropertyName(AssociateDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = AssociateDataMemberNames.UserId)]
        public long UserId { get { return _UserId; } protected set { _UserId = value; } }
        private AssociateType _AssociateType;
        [JsonPropertyName(AssociateDataMemberNames.AssociateType)]
        [JsonInclude]
        [DataMember(Name = AssociateDataMemberNames.AssociateType)]
        public AssociateType AssociateType { get { return _AssociateType; } set { _AssociateType = value; } }

        //STRICTLY NO UPDATES TO PROPERTIES. ONLY REPLACE WHOLE ASSOCIATE OBJECT
        public Associate(long userId, AssociateType associateType) {
            _UserId = userId;
            _AssociateType = associateType;
        }
        protected Associate() { }

    }
}

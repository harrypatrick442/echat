using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;

namespace Users
{
    [DataContract]
    public class Associates
    {
        private AssociateType _AssociateTypeVisibleTo;
        [JsonPropertyName(AssociatesDataMemberNames.AssociateTypeVisibleTo)]
        [JsonInclude]
        [DataMember(Name = AssociatesDataMemberNames.AssociateTypeVisibleTo)]
        public AssociateType AssociateTypeVisibleTo { get { return _AssociateTypeVisibleTo; } protected set { _AssociateTypeVisibleTo = value; } }
        public Dictionary<long, Associate> _MapUserIdToEntry;
        [JsonPropertyName(AssociatesDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name =AssociatesDataMemberNames.Entries)]
        public Associate[] Entries { 
            get { lock (this) { return _MapUserIdToEntry?.Values.ToArray(); } }
            protected set { _MapUserIdToEntry = value==null? new Dictionary<long, Associate>():value.ToDictionary(x=>x.UserId, y=>y); }
        }
        public Associates() { }
        public void Remove(long userId) {
            lock (this) {
                _MapUserIdToEntry?.Remove(userId);
            }
        }
        public Associate AddAssociate(long userId, AssociateType associateType)
        {
            lock (this)
            {
                Associate associate = new Associate(userId, associateType);
                if (_MapUserIdToEntry == null) 
                    _MapUserIdToEntry = new Dictionary<long, Associate> { { userId, associate } };
                else _MapUserIdToEntry[userId] = associate;
                return associate;
            }
        }
        public bool TryGet(long userId, out Associate associate)
        {
            lock (this)
            {
                associate = null;
                if (_MapUserIdToEntry == null) return false;
                if (_MapUserIdToEntry.TryGetValue(userId, out associate))
                    return true;
                return false;
            }
        }
        public AssociateType DowngradeAssociation(long userId, AssociateType associateTypesToKeep)
        {
            lock (this)
            {
                if (_MapUserIdToEntry == null) return AssociateType.None;
                if (!_MapUserIdToEntry.TryGetValue(userId, out Associate existing))
                    return AssociateType.None;
                existing.AssociateType = existing.AssociateType&associateTypesToKeep;
                return existing.AssociateType;
            }
        }
        public bool VisibleToMe(AssociateType myAssociateTypeWithThisUser) {
            if ((((VisibleTo)_AssociateTypeVisibleTo) & VisibleTo.Public) > 0) return true;
            return (myAssociateTypeWithThisUser & _AssociateTypeVisibleTo) > 0;
        }
    }
}

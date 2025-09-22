using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Messages;
using Core.DataMemberNames;
using Core.Timing;
using UsersEnums;
namespace Users
{
    [DataContract]
    public class AssociateRequests
    {
        private Dictionary<long, AssociateRequest> _MapUserIdToRequest;
        [JsonPropertyName(AssociateRquestsDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestsDataMemberNames.Entries)]
        public AssociateRequest[] Entries
        {
            get { lock (this) { return _MapUserIdToRequest?.Values.ToArray(); } }
            set { _MapUserIdToRequest = value?.ToDictionary(e => e.UserId, e => e); }
        }
        protected AssociateRequests() { }
        public bool TryGet(long userId, out AssociateRequest associateRequest)
        {
            lock (this)
            {
                if (_MapUserIdToRequest == null)
                {
                    associateRequest = null;
                    return false;
                }
                return _MapUserIdToRequest.TryGetValue(userId, out associateRequest);
            }
        }
        public void Remove(long userId) {
            lock (this) {
                if (_MapUserIdToRequest == null) return;
                if (!_MapUserIdToRequest.Remove(userId)) return;
                if (_MapUserIdToRequest.Any()) return;
                _MapUserIdToRequest = null;

            }
        }
        public AssociateRequest Add(long userId, long requestUniqueIdentifier, AssociateType associateType, bool isCounterRequest=false)
        {
            lock (this)
            {
                AssociateRequest associateRequest = new AssociateRequest(userId, requestUniqueIdentifier, associateType, TimeHelper.MillisecondsNow, isCounterRequest);
                if (_MapUserIdToRequest == null)
                {
                    _MapUserIdToRequest = new Dictionary<long, AssociateRequest> { {
                            userId,
                            associateRequest } };
                    return associateRequest;
                };
                _MapUserIdToRequest[userId]=associateRequest;
                return associateRequest;
            }
        }
    }
}

using Core.Messages.Messages;
using LocationCore;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserLocation.DataMemberNames.Requests;

namespace UserLocation
{
    [DataContract]
    public class UserQuadTreeSetRequest : TicketedMessageBase
    {
        [JsonPropertyName(UserQuadTreeGetRequestDataMemberNames.LatLng)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetRequestDataMemberNames.LatLng)]
        public LatLng LatLng { get; protected set; }
        public UserQuadTreeSetRequest(LatLng latLng)
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet)
        {
            LatLng = latLng;
        }
        protected UserQuadTreeSetRequest()
            : base(global::MessageTypes.MessageTypes.UserQuadTreeSet) { }
    }
}

using Core.Messages.Messages;
using LocationCore;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserLocation.DataMemberNames.Requests;

namespace UserLocation
{
    [DataContract]
    public class UserQuadTreeGetRequest:TicketedMessageBase
    {
        [JsonPropertyName(UserQuadTreeGetRequestDataMemberNames.LatLng)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetRequestDataMemberNames.LatLng)]
        public LatLng LatLng { get; protected set; }
        [JsonPropertyName(UserQuadTreeGetRequestDataMemberNames.RadiusKm)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetRequestDataMemberNames.RadiusKm)]
        public double? RadiusKm { get; protected set; }
        [JsonPropertyName(UserQuadTreeGetRequestDataMemberNames.LevelQuadrantPairs)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetRequestDataMemberNames.LevelQuadrantPairs)]
        public LevelQuadrantPair[] LevelQuadrantPairs { get; protected set; }
        public UserQuadTreeGetRequest(LatLng latLng, double radiusKm)
            : base(global::MessageTypes.MessageTypes.UserQuadTreeGet)
        {
            LatLng = latLng;
            RadiusKm = radiusKm;
        }
        protected UserQuadTreeGetRequest()
            : base(global::MessageTypes.MessageTypes.UserQuadTreeGet) { }
    }
}

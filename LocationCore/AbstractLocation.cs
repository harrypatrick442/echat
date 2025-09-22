using LocationCore.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace LocationCore
{
    [DataContract]
    public class AbstractLocation
    {
        [JsonPropertyName(AbstractLocationDataMemberNames.LatLng)]
        [JsonInclude]
        [DataMember(Name =AbstractLocationDataMemberNames.LatLng)]
        public LatLng LatLng { get; protected set; }
        [JsonPropertyName(AbstractLocationDataMemberNames.FormattedAddress)]
        [JsonInclude]
        [DataMember(Name = AbstractLocationDataMemberNames.FormattedAddress)]
        public string FormattedAddress { get; protected set; }
        [JsonPropertyName(AbstractLocationDataMemberNames.AddressComponentsSerialized)]
        [JsonInclude]
        [DataMember(Name = AbstractLocationDataMemberNames.AddressComponentsSerialized)]
        public string AddressComponentsSerialized { get; protected set; }
        protected AbstractLocation() { }
    }
}

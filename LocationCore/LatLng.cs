using LocationCore.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LocationCore
{
    [DataContract]
    public class LatLng
    {
        private double _Lat;
        [JsonPropertyName(LatLngDataMemberNames.Lat)]
        [JsonInclude]
        [DataMember(Name = LatLngDataMemberNames.Lat)]
        public double Lat { get { return _Lat; } protected set { _Lat = value; } }
        private double _Lng;
        [JsonPropertyName(LatLngDataMemberNames.Lng)]
        [JsonInclude]
        [DataMember(Name= LatLngDataMemberNames.Lng)]
        public double Lng { get { return _Lng; } protected set { _Lng = value; } }
        public LatLng(double lat, double lng){
            _Lat = lat;
            _Lng = lng;
        }
        protected LatLng() { }
    }
}

using LocationCore.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LocationCore
{
    [DataContract]
    public class Quadrant
    {
        [JsonPropertyName(QuadrantDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = QuadrantDataMemberNames.Id)]
        public long Id { get; protected set; }
        [JsonPropertyName(QuadrantDataMemberNames.Lat)]
        [JsonInclude]
        [DataMember(Name = QuadrantDataMemberNames.Lat)]
        public double Lat { get; protected set; }
        [JsonPropertyName(QuadrantDataMemberNames.Lng)]
        [JsonInclude]
        [DataMember(Name = QuadrantDataMemberNames.Lng)]
        public double Lng { get; protected set; }
        public Quadrant(long id, double lat, double lng)
        {
            Id = id;
            Lat = lat;
            Lng = lng;
        }
        protected Quadrant() { }
    }
}

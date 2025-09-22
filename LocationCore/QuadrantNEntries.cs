using LocationCore.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LocationCore
{
    [DataContract]
    public class QuadrantNEntries
    {
        [JsonPropertyName(QuadrantNEntriesDataMemberNames.NEntries)]
        [JsonInclude]
        [DataMember(Name = QuadrantNEntriesDataMemberNames.NEntries)]
        public int NEntries { get; protected set; }
        [JsonPropertyName(QuadrantNEntriesDataMemberNames.Lat)]
        [JsonInclude]
        [DataMember(Name = QuadrantNEntriesDataMemberNames.Lat)]
        public double? Lat { get; protected set; }
        [JsonPropertyName(QuadrantNEntriesDataMemberNames.Lng)]
        [JsonInclude]
        [DataMember(Name = QuadrantNEntriesDataMemberNames.Lng)]
        public double? Lng { get; protected set; }
        [JsonPropertyName(QuadrantNEntriesDataMemberNames.Quadrant)]
        [JsonInclude]
        [DataMember(Name = QuadrantNEntriesDataMemberNames.Quadrant)]
        public long Quadrant { get; protected set; }
        public QuadrantNEntries(int nEntries, long quadrant)
        {
            NEntries = nEntries;
            Quadrant = quadrant;
        }
        public QuadrantNEntries(int nEntries, double? lat, double? lng, long quadrant)
        {
            NEntries = nEntries;
            Lat = lat;
            Lng = lng;
            Quadrant = quadrant;
        }
        protected QuadrantNEntries() { }
    }
}

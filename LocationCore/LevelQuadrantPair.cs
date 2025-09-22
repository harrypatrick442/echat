using LocationCore.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LocationCore
{
    [DataContract]
    public class LevelQuadrantPair
    {
        [JsonPropertyName(LevelQuadrantPairDataMemberNames.Level)]
        [JsonInclude]
        [DataMember(Name =LevelQuadrantPairDataMemberNames.Level)]
        public int Level { get; protected set; }
        [JsonPropertyName(LevelQuadrantPairDataMemberNames.Quadrant)]
        [JsonInclude]
        [DataMember(Name = LevelQuadrantPairDataMemberNames.Quadrant)]
        public long Quadrant { get; protected set; }
        public LevelQuadrantPair(int level, long quadrant) {
            Level = level;
            Quadrant = quadrant;
        }
        protected LevelQuadrantPair() { }
    }
}

using LocationCore.DataMemberNames.Interserver.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LocationCore
{
    [DataContract]
    public class LevelQuadrantPairsForId
    {
        [JsonPropertyName(LevelQuadrantPairsForIdDataMemberNames.LevelQuadrantPairs)]
        [JsonInclude]
        [DataMember(Name = LevelQuadrantPairsForIdDataMemberNames.LevelQuadrantPairs)]
        public LevelQuadrantPair[] LevelQuadrantPairs { get; set; }
        public LevelQuadrantPairsForId(LevelQuadrantPair[] levelQuadrantPairs) { 
            LevelQuadrantPairs = levelQuadrantPairs;
        }
        protected LevelQuadrantPairsForId() { }
    }
}

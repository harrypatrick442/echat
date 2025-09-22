using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;
namespace LocationCore.DataMemberNames.Interserver.Messages
{
    public static class LevelQuadrantPairsForIdDataMemberNames
    {
        [DataMemberNamesClass(typeof(LevelQuadrantPairDataMemberNames), isArray: true)]
        public const string LevelQuadrantPairs = "l";

    }
}
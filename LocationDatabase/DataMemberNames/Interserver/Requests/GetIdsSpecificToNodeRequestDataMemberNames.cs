using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Location.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.QuadTreeGetIdsSpecificToNode)]
    public static class GetIdsSpecificToNodeRequestDataMemberNames
    {
        public const string DatabaseIdentifier = "d";
        [DataMemberNamesClass(typeof(LevelQuadrantPairDataMemberNames), isArray: true)]
        public const string LevelQuadrantPairs = "l";

    }
}
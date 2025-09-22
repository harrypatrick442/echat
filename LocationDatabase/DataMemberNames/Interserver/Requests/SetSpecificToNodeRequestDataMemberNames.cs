using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Location.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.QuadTreeSetSpecificToNode)]
    public static class SetSpecificToNodeRequestDataMemberNames
    {
        public const string DatabaseIdentifier = "d",
            Id = "i";
        [DataMemberNamesClass(typeof(LevelQuadrantPairDataMemberNames), isArray: true)]
        public const string LevelQuadrantPairs = "l";
        [DataMemberNamesClass(typeof(LatLngDataMemberNames), isArray: false)]
        public const string LatLng = "m";

    }
}
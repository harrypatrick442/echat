using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace UserLocation.DataMemberNames.Requests
{
    [MessageType(MessageTypes.UserQuadTreeGet)]
    public static class UserQuadTreeGetRequestDataMemberNames
    {
        [DataMemberNamesClass(typeof(LatLngDataMemberNames), isArray: false)]
        public const string LatLng = "l";
        public const string RadiusKm = "r";
        [DataMemberNamesClass(typeof(LevelQuadrantPairDataMemberNames), isArray: true)]
        public const string LevelQuadrantPairs = "p";

    }
}
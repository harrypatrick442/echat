using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace UserLocation.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UserQuadTreeSet)]
    public static class UserQuadTreeSetRequestDataMemberNames
    {
        [DataMemberNamesClass(typeof(LatLngDataMemberNames), isArray: false)]
        public const string LatLng = "l";
        public const string RadiusKm = "r";

    }
}
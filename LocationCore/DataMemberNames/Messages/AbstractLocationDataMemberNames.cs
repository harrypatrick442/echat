using MessageTypes.Attributes;

namespace LocationCore.DataMemberNames.Messages
{
    public class AbstractLocationDataMemberNames
    {
        [DataMemberNamesClass(typeof(LatLngDataMemberNames))]
        public const string LatLng = "l";
        public const string FormattedAddress = "f";
        public const string AddressComponentsSerialized = "a";

    }
}

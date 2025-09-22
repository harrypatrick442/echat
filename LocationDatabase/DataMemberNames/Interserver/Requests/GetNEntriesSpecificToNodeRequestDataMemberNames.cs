using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Location.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.QuadTreeGetNEntriesSpecificToNode)]
    public static class GetNEntriesSpecificToNodeRequestDataMemberNames
    {
        public const string DatabaseIdentifier = "d";
        public const string Level = "l";
        public const string Quadrants = "q";
        public const string WithLatLng = "w";
    }
}
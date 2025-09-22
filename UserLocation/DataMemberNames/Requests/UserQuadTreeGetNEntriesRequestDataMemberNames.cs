using MessageTypes.Attributes;

namespace UserLocation.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.UserQuadTreeGetNEntries)]
    public static class UserQuadTreeGetNEntriesRequestDataMemberNames
    {
        public const string Level = "l";
        public const string Quadrants = "q";
        public const string WithLatLng = "w";
    }
}
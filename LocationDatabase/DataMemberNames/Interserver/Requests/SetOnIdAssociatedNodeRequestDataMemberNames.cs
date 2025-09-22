using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Location.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.QuadTreeSetOnIdAssociatedNode)]
    public static class SetOnIdAssociatedNodeRequestDataMemberNames
    {
        public const string DatabaseIdentifier = "d",
            Id = "i";
        [DataMemberNamesClass(typeof(LatLngDataMemberNames))]
        public const string LatLng = "l";

    }
}
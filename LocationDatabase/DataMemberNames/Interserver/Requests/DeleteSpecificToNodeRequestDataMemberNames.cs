using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Location.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.QuadTreeDeleteSpecificToNode)]
    public static class DeleteSpecificToNodeRequestDataMemberNames
    {
        public const string DatabaseIdentifier = "d",
            Id = "i",
            Levels = "l";

    }
}
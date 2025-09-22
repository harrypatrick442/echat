using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Location.DataMemberNames.Interserver.Requests
{
    [MessageType(InterserverMessageTypes.QuadTreeDeleteOnIdAssociatedNode)]
    public static class DeleteOnIdAssociatedNodeRequestDataMemberNames
    {
        public const string DatabaseIdentifier = "d",
            Id = "i";

    }
}
using MessageTypes.Attributes;
using MessageTypes.Internal;

namespace Flagging.DataMemberNames.Requests
{
    [MessageType(MessageTypes.FlaggingFlag)]
    public static class FlagRequestDataMemberNames
    {
        [DataMemberNamesIgnore(toJSON:true)]
        public const string
            UserIdFlagging = "b";
        public const string
            UserIdBeingFlagged = "c",
            FlagType = "d",
            ScopeType = "e",
            ScopeId = "f",
            Description = "i",
            ScopeId2 = "g";
        [DataMemberNamesIgnore(toJSON: true)]
        public const string
            FlaggedAt = "h";
    }
}
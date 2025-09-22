using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Location.DataMemberNames.Interserver.Responses
{
    public static class GetNEntriesSpecificToNodeResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(QuadrantNEntriesDataMemberNames), isArray: true)]
        public const string QuadrantNEntriess = "q";
        public const string Successful = "s";
    }
}
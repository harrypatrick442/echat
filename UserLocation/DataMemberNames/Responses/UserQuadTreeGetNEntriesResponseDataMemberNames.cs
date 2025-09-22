using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace UserLocation.DataMemberNames.Responses
{
    public static class UserQuadTreeGetNEntriesResponseDataMemberNames
    {
        public const string Success = "s";
        [DataMemberNamesClass(typeof(QuadrantNEntriesDataMemberNames), isArray: true)]
        public const string QuadrantNEntriess = "q";

    }
}
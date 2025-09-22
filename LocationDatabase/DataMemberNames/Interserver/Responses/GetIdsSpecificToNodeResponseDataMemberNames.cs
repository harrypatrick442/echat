using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Location.DataMemberNames.Interserver.Responses
{
    public static class GetIdsSpecificToNodeResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(QuadrantDataMemberNames), isArray: true)]
        public const string Quadrants = "q";
        public const string Successful = "s";
    }
}
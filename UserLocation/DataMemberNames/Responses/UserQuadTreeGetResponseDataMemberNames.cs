using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace UserLocation.DataMemberNames.Responses
{
    public static class UserQuadTreeGetResponseDataMemberNames
    {
        public const string Success = "s";
        [DataMemberNamesClass(typeof(QuadrantDataMemberNames), isArray: true)]
        public const string Quadrants = "q";

    }
}
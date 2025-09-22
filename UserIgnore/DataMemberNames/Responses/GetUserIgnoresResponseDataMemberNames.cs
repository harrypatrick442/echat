using MessageTypes.Attributes;
using UserIgnore.DataMemberNames.Messages;

namespace UserIgnore.DataMemberNames.Responses
{
    public static class GetUserIgnoresResponseDataMemberNames
    {
        public const string Success = "s";
        [DataMemberNamesClass(typeof(UserIgnoresDataMemberNames))]
        public const string UserIgnores = "u";
    }
}
using Administration.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class GetAdministratorsResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(AdministratorDataMemberNames), isArray: true)]
        public const string Administrators = "a";
        public const string FailedReason = "f";
    }
}
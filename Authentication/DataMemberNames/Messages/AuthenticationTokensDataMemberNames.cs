using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Messages
{
    public class AuthenticationTokensDataMemberNames
    {
        [DataMemberNamesClass(typeof(AuthenticationTokenDataMemberNames), isArray:true)]
        public const string Entries = "e";
    }
}
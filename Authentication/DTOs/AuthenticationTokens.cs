using Authentication.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.DTOs
{
    [DataContract]
    [KnownType(typeof(AuthenticationToken))]
    public class AuthenticationTokens
    {
        private AuthenticationToken[] _Entries;
        [JsonPropertyName(AuthenticationTokensDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name =AuthenticationTokensDataMemberNames.Entries)]
        public AuthenticationToken[] Entries { get { return _Entries; } protected set { _Entries = value; } }
        public AuthenticationTokens(AuthenticationToken[] entries) {
            _Entries = entries;
        }
        protected AuthenticationTokens() { }
    }
}

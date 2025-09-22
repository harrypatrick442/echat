using Core.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Sessions;
using Authentication.DataMemberNames.Requests;

namespace Authentication.Messages
{
    [DataContract]
    public class TokenAuthenticationResult
    {
        [JsonPropertyName(global::MessageTypes.MessageTypes.Type)]
        [JsonInclude]
        [DataMember(Name = global::MessageTypes.MessageTypes.Type)]

        public string Type { get { return global::MessageTypes.MessageTypes.TokenAuthenticationResult; } protected set { } }
        [JsonPropertyName(TokenAuthenticationResultDataMemberNames.Successful)]
        [JsonInclude]
        [DataMember(Name = TokenAuthenticationResultDataMemberNames.Successful)]
        public bool Successful { get; protected set; }
        [JsonPropertyName(TokenAuthenticationResultDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = TokenAuthenticationResultDataMemberNames.UserId, EmitDefaultValue = false)]
        public long? UserId { get; protected set; }
        [JsonPropertyName(TokenAuthenticationResultDataMemberNames.Iat)]
        [JsonInclude]
        [DataMember(Name = TokenAuthenticationResultDataMemberNames.Iat, EmitDefaultValue = false)]
        public string Iat { get; protected set; }

        private AuthenticationFailedReason? _AuthenticationFailedReason;

        [JsonPropertyName(TokenAuthenticationResultDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = TokenAuthenticationResultDataMemberNames.FailedReason, EmitDefaultValue = false)]
        public AuthenticationFailedReason? AuthenticationFailedReason { get { return _AuthenticationFailedReason; } protected set { _AuthenticationFailedReason = value; } }
        [JsonPropertyName(TokenAuthenticationResultDataMemberNames.AdditionalPayload)]
        [JsonInclude]
        [DataMember(Name = TokenAuthenticationResultDataMemberNames.AdditionalPayload, EmitDefaultValue = false)]
        public object AdditionalPayload { get; protected set; }
        public TokenAuthenticationResult(
            bool successful, AuthenticationFailedReason? authenticationFailedReason,
            long? userId, string iat, object additionalPayload)
        {
            Successful = successful;
            AuthenticationFailedReason = authenticationFailedReason;
            UserId = userId;
            Iat = iat;
            AdditionalPayload = additionalPayload;
        }
        protected TokenAuthenticationResult()
        {

        }
        public static TokenAuthenticationResult Failed(AuthenticationFailedReason authenticationFailedReason)
        {
            return new TokenAuthenticationResult(false, authenticationFailedReason, null, null, null);
        }
        public static TokenAuthenticationResult Success(
            long userId, long sessionId, string token, object additionalPayload)
        {
            string iat = new InterserverAuthenticationToken(Nodes.Nodes.Instance.MyId, sessionId, token).ToString();
            return new TokenAuthenticationResult(true, null, userId, iat, additionalPayload);
        }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Enums;
using System.Security.Authentication;
using Core.Exceptions;
using Sessions;
using Authentication.DataMemberNames.Responses;

namespace Authentication.Responses
{
    [DataContract]
    public class AuthenticateResponse : Core.Messages.Responses.TicketedResponseMessageBase
    {
        private AuthenticationFailedReason? _AuthenticationFailedReason;
        [JsonPropertyName(AuthenticateResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.FailedReason, EmitDefaultValue = false)]
        /*public AuthenticationFailedReason? AuthenticationFailedReason
        {
            get { return _AuthenticationFailedReason; }
            protected set { _AuthenticationFailedReason = value; }
        }*/

        public int AuthenticationFailedReasonInt
        {
            get { return _AuthenticationFailedReason == null ? 0 : (int)_AuthenticationFailedReason; }
            set { _AuthenticationFailedReason = value == 0 ? AuthenticationFailedReason.Unknown : (AuthenticationFailedReason)value; }
        }
        [JsonPropertyName(AuthenticateResponseDataMemberNames.Token)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.Token)]
        public string Token { get; protected set; }
        [JsonPropertyName(AuthenticateResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.Success)]
        public bool Success { get { return _AuthenticationFailedReason == null; } set { } }
        private int? _DelayRetrySeconds;
        [JsonPropertyName(AuthenticateResponseDataMemberNames.DelayRetrySeconds)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.DelayRetrySeconds, EmitDefaultValue = false)]
        public int? DelayRetrySeconds { get { return _DelayRetrySeconds; } protected set { } }
        private int _SubReason;
        [JsonPropertyName(AuthenticateResponseDataMemberNames.SubReason)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.SubReason, EmitDefaultValue = false)]
        public int SubReason { get { return _SubReason; } protected set { } }
        [JsonPropertyName(AuthenticateResponseDataMemberNames.MinValue)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.MinValue, EmitDefaultValue = false)]
        public int? MinValue { get; protected set; }
        [JsonPropertyName(AuthenticateResponseDataMemberNames.MaxValue)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.MaxValue, EmitDefaultValue = false)]
        public int? MaxValue { get; protected set; }
        [JsonPropertyName(AuthenticateResponseDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(AuthenticateResponseDataMemberNames.Iat)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.Iat)]
        public string Iat { get; protected set; }
        [JsonPropertyName(AuthenticateResponseDataMemberNames.AdditionalPayload)]
        [JsonInclude]
        [DataMember(Name = AuthenticateResponseDataMemberNames.AdditionalPayload, EmitDefaultValue = false)]
        public object AdditionalPayload { get; protected set; }
        protected AuthenticateResponse(long ticket, long userId, string token,
            object additionalPayloadSerialized,
            AuthenticationFailedReason? failedReason, int? delayRetrySeconds,
            int subReason, int? minValue = null, int? maxValue = null, string iat = null) : base(ticket)
        {
            Token = token;
            AdditionalPayload = additionalPayloadSerialized;
            _AuthenticationFailedReason = failedReason;
            _DelayRetrySeconds = delayRetrySeconds;
            _SubReason = subReason;
            MinValue = minValue;
            MaxValue = maxValue;
            UserId = userId;
            Iat = iat;
        }
        protected AuthenticateResponse() : base(0)
        {

        }

        public Exception GetException()
        {
            if (_AuthenticationFailedReason == null) return null;
            switch ((AuthenticationFailedReason)_AuthenticationFailedReason)
            {
                case AuthenticationFailedReason.BadCredentials:
                    return new BadCredentialsException("bad credentials");
                case AuthenticationFailedReason.TooManyAttempts:
                    return new MustWaitToRetryException($"Too many attempts. You must wait" + (_DelayRetrySeconds == null ? "" : $"{(int)_DelayRetrySeconds} seconds"), _DelayRetrySeconds == null ? 0 : (int)_DelayRetrySeconds);
                case AuthenticationFailedReason.Busy:
                    return new BusyException("Server too busy");
                case AuthenticationFailedReason.UserId:
                    return new InvalidCredentialException("Invalid user id");
                case AuthenticationFailedReason.Phone:
                    return new InvalidCredentialException("Invalid phone");
                case AuthenticationFailedReason.Email:
                    return new InvalidCredentialException("Invalid email");
                case AuthenticationFailedReason.Password:
                    return new InvalidCredentialException("Invalid password");
                case AuthenticationFailedReason.Unknown:
                    return new Exception("Unkmown server error");
                default:
                    return new Exception("Unkmown server error");
            }
        }
        public static AuthenticateResponse Successful(
            long ticket, long userId, string token, long sessionId,
            object additionalPayloadSerialized)
        {
            string iat = new InterserverAuthenticationToken(Nodes.Nodes.Instance.MyId, sessionId, token).ToString();
            return new AuthenticateResponse(ticket, userId, token, additionalPayloadSerialized, null, null, 0, iat:iat);
        }
        public static AuthenticateResponse Failed(long ticket,
            AuthenticationFailedReason? failedReason, int? delayRetrySeconds, int subReason, int? minValue = null, int? maxValue = null)
        {
            return new AuthenticateResponse(ticket, 0, null, null, failedReason, delayRetrySeconds, subReason, minValue, maxValue);
        }
    }
}

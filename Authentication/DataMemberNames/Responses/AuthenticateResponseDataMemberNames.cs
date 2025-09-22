using MessageTypes.Attributes;

namespace Authentication.DataMemberNames.Responses
{
    public static class AuthenticateResponseDataMemberNames
    {
        public const string FailedReason = "f";
        public const string Token = "t";
        public const string Success = "s";
        public const string DelayRetrySeconds = "d";
        public const string SubReason = "sr";
        public const string MinValue = "mn";
        public const string MaxValue = "mx";
        public const string UserId = "u";
        public const string Iat = "iat";
        public const string AdditionalPayload = "a";
    }
}
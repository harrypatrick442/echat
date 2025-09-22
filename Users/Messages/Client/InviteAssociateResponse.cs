using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Responses;
using Users.Enums;

namespace Users.Messages.Client
{
    [DataContract]
    public class InviteAssociateResponse
    {
        private InviteFailedReason? _FailedReason;
        [JsonPropertyName(InviteAssociateResponseDataMemberNames.FailedReason)]
        [JsonInclude]
        [DataMember(Name = InviteAssociateResponseDataMemberNames.FailedReason,
            EmitDefaultValue = false)]
        public InviteFailedReason? FailedReason { get { return _FailedReason; } protected set { _FailedReason = value; } }
        [JsonPropertyName(InviteAssociateResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = InviteAssociateResponseDataMemberNames.Success)]
        public bool Success { get { return _FailedReason == null; } set { } }
        private int? _DelayRetrySeconds;
        [JsonPropertyName(InviteAssociateResponseDataMemberNames.DelayRetrySeconds)]
        [JsonInclude]
        [DataMember(Name = InviteAssociateResponseDataMemberNames.DelayRetrySeconds,
            EmitDefaultValue = false)]
        public int? DelayRetrySeconds { get { return _DelayRetrySeconds; } protected set { } }
        protected InviteAssociateResponse(InviteFailedReason? failedReason,
            int? delayRetrySeconds)
        {
            _FailedReason = failedReason;
            _DelayRetrySeconds = delayRetrySeconds;
        }
        protected InviteAssociateResponse() { }

        public static InviteAssociateResponse Successful()
        {
            return new InviteAssociateResponse(null, 0);
        }
        public static InviteAssociateResponse Failed(
            InviteFailedReason failedReason, int? delayRetrySeconds)
        {
            return new InviteAssociateResponse(failedReason, delayRetrySeconds);
        }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Authentication.DataMemberNames.Responses;
using Core.Messages.Responses;

namespace Authentication.Responses
{
    [DataContract]
    public class ResetPasswordByEmailResponse : TicketedResponseMessageBase
    {
        [JsonPropertyName(ResetPasswordByEmailResponseDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = ResetPasswordByEmailResponseDataMemberNames.Success,
            EmitDefaultValue = false)]

        public bool Success { get; set; }
        protected ResetPasswordByEmailResponse():base(0)
        {

        }
        public ResetPasswordByEmailResponse(bool success, long ticket):base(ticket)
        {
            Success = success;
        }
    }
}

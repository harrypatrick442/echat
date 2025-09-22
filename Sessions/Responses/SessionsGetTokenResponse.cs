using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Responses;
using Sessions.DataMemberNames.Interserver.Responses;

namespace Sessions.Responses
{
    [DataContract]
    public class SessionsGetTokenResponse : TicketedResponseMessageBase
    {
        [JsonPropertyName(SessionsGetTokenResponseDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = SessionsGetTokenResponseDataMemberNames.UserId, EmitDefaultValue = false)]
        public long? UserId
        {
            get;
            protected set;
        }
        public SessionsGetTokenResponse(long? userId, long ticket) : base(ticket)
        {
            UserId = userId;
        }
        protected SessionsGetTokenResponse() : base(0) { }
    }
}

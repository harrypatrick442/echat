using Core.Messages.Messages;
using MessageTypes.Internal;
using Sessions.DataMemberNames.Interserver.Requests;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sessions.Requests
{
    [DataContract]
    public class SessionsGetTokenRequest: TicketedMessageBase
    {
        [JsonPropertyName(SessionsGetTokenRequestDataMemberNames.SessionId)]
        [JsonInclude]
        [DataMember(Name = SessionsGetTokenRequestDataMemberNames.SessionId)]
        public long SessionId { get; }
        [JsonPropertyName(SessionsGetTokenRequestDataMemberNames.Token)]
        [JsonInclude]
        [DataMember(Name = SessionsGetTokenRequestDataMemberNames.Token)]
        public string Token { get; }
        public SessionsGetTokenRequest(long sessionId, string token) : base(InterserverMessageTypes.SessionsAuthenticate)
        {
            SessionId = sessionId;
            Token = token;
        }
        protected SessionsGetTokenRequest() : base(InterserverMessageTypes.SessionsAuthenticate) { }
    }
}

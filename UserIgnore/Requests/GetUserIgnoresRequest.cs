using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Requests;

namespace UserIgnore.Requests
{
    [DataContract]
    public class GetUserIgnoresRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetUserIgnoresRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = GetUserIgnoresRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public GetUserIgnoresRequest(long userId)
            : base(global::MessageTypes.MessageTypes.UserIgnoreGet)
        {
            UserId = userId;
        }
        protected GetUserIgnoresRequest()
            : base(global::MessageTypes.MessageTypes.UserIgnoreGet) { }
    }
}

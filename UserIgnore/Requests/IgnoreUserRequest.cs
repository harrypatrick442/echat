using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Requests;

namespace UserIgnore.Requests
{
    [DataContract]
    public class IgnoreUserRequest : TicketedMessageBase
    {
        [JsonPropertyName(IgnoreUserRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = IgnoreUserRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public IgnoreUserRequest(long userId)
            : base(MessageTypes.UserIgnoreIgnore)
        {
            UserId = userId;
        }
        protected IgnoreUserRequest()
            : base(MessageTypes.UserIgnoreIgnore) { }
    }
}

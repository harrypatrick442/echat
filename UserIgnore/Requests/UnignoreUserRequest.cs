using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Requests;

namespace UserIgnore.Requests
{
    [DataContract]
    public class UnignoreUserRequest : TicketedMessageBase
    {
        [JsonPropertyName(UnignoreUserRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UnignoreUserRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public UnignoreUserRequest(long userId)
            : base(global::MessageTypes.MessageTypes.UserIgnoreUnignore)
        {
            UserId = userId;
        }
        protected UnignoreUserRequest()
            : base(global::MessageTypes.MessageTypes.UserIgnoreUnignore) { }
    }
}

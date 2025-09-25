using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class UnbanUserRequest : TicketedMessageBase
    {
        [JsonPropertyName(UnbanUserRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UnbanUserRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public UnbanUserRequest(long conversationId, long userId)
            : base(MessageTypes.ChatUnbanUser)
        {
            UserId = userId;
        }
        protected UnbanUserRequest()
            : base(MessageTypes.ChatUnbanUser) { }
    }
}

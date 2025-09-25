using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Requests;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class BanUserRequest : TicketedMessageBase
    {
        [JsonPropertyName(BanUserRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = BanUserRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public BanUserRequest(long conversationId, long userId)
            : base(MessageTypes.ChatBanUser)
        {
            UserId = userId;
        }
        protected BanUserRequest()
            : base(MessageTypes.ChatBanUser) { }
    }
}

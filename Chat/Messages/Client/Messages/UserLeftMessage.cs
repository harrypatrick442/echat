using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class UserLeftMessage : TypedMessageBase
    {
        [JsonPropertyName(UserLeftMessageDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserLeftMessageDataMemberNames.UserId)]
        public long UserId
        {
            get;
            protected set;
        }
        public UserLeftMessage(long userId) : base()
        {
            _Type = MessageTypes.ChatRoomUserLeft;
            UserId = userId;
        }
        protected UserLeftMessage() { }
    }
}

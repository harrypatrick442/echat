using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class UserJoinedMessage : TypedMessageBase
    {
        [JsonPropertyName(UserJoinedMessageDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserJoinedMessageDataMemberNames.UserId)]
        public long UserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(UserJoinedMessageDataMemberNames.Timestamp)]
        [JsonInclude]
        [DataMember(Name = UserJoinedMessageDataMemberNames.Timestamp)]
        public long Timestamp
        {
            get;
            protected set;
        }
        public UserJoinedMessage(long userId, long timestamp) : base()
        {
            _Type = MessageTypes.ChatRoomUserJoined;
            UserId = userId;
            Timestamp = timestamp;
        }
        protected UserJoinedMessage() { }
    }
}

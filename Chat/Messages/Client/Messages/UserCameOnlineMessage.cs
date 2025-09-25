using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class UserCameOnlineMessage : TypedMessageBase
    {
        [JsonPropertyName(UserCameOnlineMessageDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserCameOnlineMessageDataMemberNames.UserId)]
        public long UserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(UserCameOnlineMessageDataMemberNames.Timestamp)]
        [JsonInclude]
        [DataMember(Name = UserCameOnlineMessageDataMemberNames.Timestamp)]
        public long Timestamp
        {
            get;
            protected set;
        }
        public UserCameOnlineMessage(long userId, long timestamp) : base()
        {
            _Type = MessageTypes.ChatRoomUserCameOnline;
            UserId = userId;
            Timestamp = timestamp;
        }
        protected UserCameOnlineMessage() { }
    }
}

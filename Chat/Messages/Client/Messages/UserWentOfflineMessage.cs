using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class UserWentOfflineMessage : TypedMessageBase
    {
        [JsonPropertyName(UserWentOfflineMessageDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserWentOfflineMessageDataMemberNames.UserId)]
        public long UserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(UserWentOfflineMessageDataMemberNames.Timestamp)]
        [JsonInclude]
        [DataMember(Name = UserWentOfflineMessageDataMemberNames.Timestamp)]
        public long Timestamp
        {
            get;
            protected set;
        }
        public UserWentOfflineMessage(long userId, long timestamp)
        {
            _Type = global::MessageTypes.MessageTypes.ChatRoomUserWentOffline;
            UserId = userId;
            Timestamp = timestamp;
        }
        protected UserWentOfflineMessage() { }
    }
}

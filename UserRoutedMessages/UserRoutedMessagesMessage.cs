using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UserRoutedMessages
{
    [DataContract]
    public class UserRoutedMessagesMessage
    {
        [JsonPropertyName(UserRoutedMessagesMessageDataMemberNames.UserIds)]
        [JsonInclude]
        [DataMember(Name = UserRoutedMessagesMessageDataMemberNames.UserIds)]
        public long[] UserIds { get; protected set; }
        [JsonPropertyName(UserRoutedMessagesMessageDataMemberNames.SerializedMessage)]
        [JsonInclude]
        [DataMember(Name = UserRoutedMessagesMessageDataMemberNames.SerializedMessage)]
        public string SerializedMessage { get; protected set; }
        public UserRoutedMessagesMessage(long[] userIds, string serializedMessage)
        {
            UserIds = userIds;
            SerializedMessage = serializedMessage;
        }
        protected UserRoutedMessagesMessage() { }
    }
}

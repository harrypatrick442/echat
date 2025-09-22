using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class MessageChildConversationOptions
    {
        [JsonPropertyName(MessageChildConversationOptionsDataMemberNames.NMessagesReturn)]
        [JsonInclude]
        [DataMember(Name = MessageChildConversationOptionsDataMemberNames.NMessagesReturn)]
        public int? NMessagesReturn
        {
            get;
            set;
        }
        [JsonPropertyName(MessageChildConversationOptionsDataMemberNames.FromStartElseEnd)]
        [JsonInclude]
        [DataMember(Name = MessageChildConversationOptionsDataMemberNames.FromStartElseEnd)]
        public bool FromStartElseEnd
        {
            get;
            set;
        }
    }
}

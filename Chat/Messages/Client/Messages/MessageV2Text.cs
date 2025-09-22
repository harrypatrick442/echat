using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class MessageV2Text : TypedMessageBase
    {
        [JsonPropertyName(MessageV2ContentDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = MessageV2ContentDataMemberNames.Entries)]
        public int Entries
        {
            get;
            protected set;
        }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class ModifyMessage : TicketedMessageBase
    {
        [JsonPropertyName(ModifyMessageDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ModifyMessageDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(ModifyMessageDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = ModifyMessageDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        [JsonPropertyName(ModifyMessageDataMemberNames.Message)]
        [JsonInclude]
        [DataMember(Name = ModifyMessageDataMemberNames.Message)]
        public ClientMessage Message
        {
            get;
            protected set;
        }
        public ModifyMessage(long conversationId, ClientMessage message)
            : base(global::MessageTypes.MessageTypes.ChatModifyMessage)
        {
            ConversationId = conversationId;
            Message = message;
        }
        protected ModifyMessage()
            : base(global::MessageTypes.MessageTypes.ChatModifyMessage) { }
    }
}

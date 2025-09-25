using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Security.Cryptography.X509Certificates;
using Core.Messages.Messages;
using Chat.DataMemberNames.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class DeletedMessages : TypedMessageBase
    {
        [JsonPropertyName(DeletedMessagesDataMemberNames.Ids)]
        [JsonInclude]
        [DataMember(Name = DeletedMessagesDataMemberNames.Ids)]
        public long[] DeletedIds { get; protected set; }
        [JsonPropertyName(DeletedMessagesDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = DeletedMessagesDataMemberNames.ConversationId, EmitDefaultValue =false)]
        public long? ConversationId { get; protected set; }
        public DeletedMessages(long[] deletedIds, long? conversationId)
        {
            _Type = MessageTypes.ChatDeleteMessages;
            DeletedIds = deletedIds;
            ConversationId = conversationId;
        }
        protected DeletedMessages() { }
    }
}

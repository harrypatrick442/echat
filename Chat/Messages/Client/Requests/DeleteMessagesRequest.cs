using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Chat.DataMemberNames.Requests;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class DeleteMessagesRequest : TicketedMessageBase
    {
        [JsonPropertyName(DeleteMessagesRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = DeleteMessagesRequestDataMemberNames.UserId)]
        public long UserId
        {
            get;
            set;
        }
        [JsonPropertyName(DeleteMessagesRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = DeleteMessagesRequestDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(DeleteMessagesRequestDataMemberNames.MessageIds)]
        [JsonInclude]
        [DataMember(Name = DeleteMessagesRequestDataMemberNames.MessageIds)]
        public long[] MessageIds
        {
            get;
            protected set;
        }
        [JsonPropertyName(DeleteMessagesRequestDataMemberNames.CanDeleteAnyMessage)]
        [JsonInclude]
        [DataMember(Name = DeleteMessagesRequestDataMemberNames.CanDeleteAnyMessage)]
        public bool CanDeleteAnyMessage
        {
            get;
            protected set;
        }
        [JsonPropertyName(DeleteMessagesRequestDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = DeleteMessagesRequestDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        public DeleteMessagesRequest(long userId, long conversationId, long[] messageIds, bool canDeleteAnyMessage)
            : base(global::MessageTypes.MessageTypes.ChatDeleteMessages)
        {
            UserId = userId;
            ConversationId = conversationId;
            MessageIds = messageIds;
            CanDeleteAnyMessage = canDeleteAnyMessage;
        }
        protected DeleteMessagesRequest()
            : base(global::MessageTypes.MessageTypes.ChatDeleteMessages) { }
    }
}

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;
using MultimediaCore;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class ClientMessage : TypedMessageBase
    {
        [JsonPropertyName(ClientMessageDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.Id)]
        public long Id
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.Version)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.Version)]
        public int Version
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.Content)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.Content)]
        public string Content
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.SentAt)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.SentAt)]
        public long SentAt
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.UserId)]
        public long UserId
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.Tags)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.Tags)]
        public string[] Tags
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.MentionUserIds)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.MentionUserIds)]
        public long[] MentionUserIds
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.ConversationId, EmitDefaultValue = false)]
        public long ConversationId
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        [JsonPropertyName(ClientMessageDataMemberNames.ReplyTo)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.ReplyTo, EmitDefaultValue = false)]
        public long? ReplyTo
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.ReplyMessage)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.ReplyMessage, EmitDefaultValue = false)]
        public ClientMessage ReplyMessage
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.UserMultimediaItems)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.UserMultimediaItems, EmitDefaultValue = false)]
        public UserMultimediaItem[] UserMultimediaItems
        {
            get;
            set;
        }
        public bool Deleted
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.ChildConversationId)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.ChildConversationId)]
        public long ChildConversationId
        {
            get;
            set;
        }
        [JsonPropertyName(ClientMessageDataMemberNames.NChildMessages)]
        [JsonInclude]
        [DataMember(Name = ClientMessageDataMemberNames.NChildMessages)]
        public int? NChildMessages
        {
            get;
            set;
        }
        public ClientMessage(long id, int version, long sentAt, long userId, string content, ClientMessage replyMessage) : this( id,  version,  sentAt,  userId,  content)
        {
            ReplyMessage = replyMessage;
        }
        public ClientMessage(long id, int version, long sentAt, long userId, string content) : base()
        {
            _Type = MessageTypes.ClientMessage;
            Id = id;
            Version = version;
            SentAt = sentAt;
            UserId = userId;
            Content = content;
        }
        protected ClientMessage() { }
    }
}

using Chat.DataMemberNames.Messages;
using MultimediaServerCore.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;

namespace Chat.Messages.Client
{
    [DataContract]
    public class MessageUserMultimediaItem 
    {
        [JsonPropertyName(MessageUserMultimediaItemDataMemberNames.MessageId)]
        [JsonInclude]
        [DataMember(Name = MessageUserMultimediaItemDataMemberNames.MessageId)]
        public long MessageId { get; protected set; }
        [JsonPropertyName(MessageUserMultimediaItemDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = MessageUserMultimediaItemDataMemberNames.MultimediaToken)]
        public string MultimediaToken { get; protected set; }
        [JsonPropertyName(MessageUserMultimediaItemDataMemberNames.XRating)]
        [JsonInclude]
        [DataMember(Name = MessageUserMultimediaItemDataMemberNames.XRating, EmitDefaultValue = false)]
        public XRating? XRating { get; protected set; }
        [JsonPropertyName(MessageUserMultimediaItemDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = MessageUserMultimediaItemDataMemberNames.Description, EmitDefaultValue = false)]
        public string Description { get; set; }
        [JsonPropertyName(MessageUserMultimediaItemDataMemberNames.Status)]
        [JsonInclude]
        [DataMember(Name = MessageUserMultimediaItemDataMemberNames.Status, EmitDefaultValue = false)]
        public MultimediaItemStatus Status { get; set; }
        public MessageUserMultimediaItem(long messageId, 
            string multimediaToken, MultimediaItemStatus status, XRating? xRating,
            string description)
        {
            MessageId = messageId;
            MultimediaToken = multimediaToken;
            Status = status;
            XRating = xRating;
            Description = description;
        }
        protected MessageUserMultimediaItem() { }
    }
}

using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
using UsersEnums;
using Core.DataMemberNames;
using Chat.DataMemberNames.Requests;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class UploadPmMessagePictureRequest : UploadMessagePictureRequest
    {
        [JsonPropertyName(UploadPmMessagePictureRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = UploadPmMessagePictureRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(UploadPmMessagePictureRequestDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = UploadPmMessagePictureRequestDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        public UploadPmMessagePictureRequest() : base() { 
            
        }
    }
}

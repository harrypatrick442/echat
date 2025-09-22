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
    public class UploadPmMessageVideoRequest : UploadMessageVideoRequest
    {
        [JsonPropertyName(UploadPmMessageVideoRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = UploadPmMessageVideoRequestDataMemberNames.ConversationId)]
        public long ConversationId { get; protected set; }
        [JsonPropertyName(UploadPmMessageVideoRequestDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = UploadPmMessageVideoRequestDataMemberNames.ConversationType)]
        public ConversationType ConversationType { get; protected set; }
        public UploadPmMessageVideoRequest() : base() {
            
        }
    }
}

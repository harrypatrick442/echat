using Core.Messages.Messages;
using MultimediaServerCore.DataMemberNames.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
namespace MultimediaServerCore.Messages
{
    [DataContract]
    public class FailedMultimediaUpload : TypedMessageBase
    {
        [JsonPropertyName(FailedMultimediaUploadDataMemberNames.Token)]
        [JsonInclude]
        [DataMember(Name=FailedMultimediaUploadDataMemberNames.Token)]
        public string Token { get; protected set; }
        [JsonPropertyName(FailedMultimediaUploadDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name=FailedMultimediaUploadDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public FailedMultimediaUpload(
            string token, long userId) : base()
        {
            Token = token;
            UserId = userId;
            Type = global::MessageTypes.MessageTypes.MultimediaUploadFailed;
        }
        protected FailedMultimediaUpload() { }
    }
}

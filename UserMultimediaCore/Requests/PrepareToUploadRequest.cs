using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
using MessageTypes.Internal;
using MultimediaServerCore.DataMemberNames.Requests;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class PrepareToUploadRequest : TicketedMessageBase
    {
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.NodeIdRequestingUpload)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.NodeIdRequestingUpload)]
        public int NodeIdRequestingUpload { get; protected set; }
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.FileInfo)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.FileInfo)]
        public FileInfo FileInfo { get; protected set; }
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.MultimediaType)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.MultimediaType)]
        public MultimediaType MultimediaType { get; protected set; }
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.ScopingId)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.ScopingId)]
        public long UserId { get; protected set; }
        public PrepareToUploadRequest(int nodeIdRequestingUpload, MultimediaType multimediaType,
            FileInfo fileInfo, long userId) : base(InterserverMessageTypes.MultimediaPrepareToUpload)
        {
            NodeIdRequestingUpload = nodeIdRequestingUpload;
            FileInfo = fileInfo;
            MultimediaType = multimediaType;
            UserId = userId;
        }
        protected PrepareToUploadRequest() : base(InterserverMessageTypes.MultimediaPrepareToUpload)
        { }
    }
}

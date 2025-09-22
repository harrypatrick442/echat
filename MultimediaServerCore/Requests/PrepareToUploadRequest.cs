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
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.ScopeType)]
        public MultimediaScopeType ScopeType { get; protected set; }
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.ScopingId)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.ScopingId)]
        public long ScopingId { get; protected set; }
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.ScopingId2)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.ScopingId2)]
        public long? ScopingId2 { get; protected set; }
        [JsonPropertyName(PrepareToUploadRequestDataMemberNames.ScopingId3)]
        [JsonInclude]
        [DataMember(Name = PrepareToUploadRequestDataMemberNames.ScopingId3)]
        public long? ScopingId3 { get; protected set; }
        public PrepareToUploadRequest(int nodeIdRequestingUpload, MultimediaType multimediaType, 
            FileInfo fileInfo, MultimediaScopeType scopeType, long scopingId, long? scopingId2,
            long? scopingId3) : base(InterserverMessageTypes.MultimediaPrepareToUpload)
        {
            NodeIdRequestingUpload = nodeIdRequestingUpload;
            FileInfo = fileInfo;
            MultimediaType = multimediaType;
            ScopeType = scopeType;
            ScopingId = scopingId;
            ScopingId2 = scopingId2;
            ScopingId3 = scopingId3;
        }
        protected PrepareToUploadRequest() : base(InterserverMessageTypes.MultimediaPrepareToUpload) { }
    }
}

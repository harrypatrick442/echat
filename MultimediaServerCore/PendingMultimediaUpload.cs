using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
namespace MultimediaServerCore
{
    [DataContract]
    public class PendingMultimediaUpload
    {

        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.NodeIdRequestingUpload)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.NodeIdRequestingUpload)]
        public int NodeIdRequestingUpload { get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.MultimediaType)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.MultimediaType)]
        public MultimediaType MultimediaType { get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.FileInfo)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.FileInfo)]
        public FileInfo FileInfo { get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.FilePath)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.FilePath)]
        public string DirectoryPath{ get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.MultimediaToken)]
        public string MultimediaToken{ get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.ScopeType)]
        public MultimediaScopeType ScopeType { get; protected set; }//UserId //ConversationId
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.ScopingId)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.ScopingId)]
        public long ScopingId { get; protected set; }//UserId //ConversationId
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.ScopingId2)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.ScopingId2)]
        public long? ScopingId2 { get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.ScopingId3)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.ScopingId3)]
        public long? ScopingId3 { get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.Extension)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.Extension)]
        public string Extension { get; protected set; }
        [JsonPropertyName(PendingMultimediaUploadDataMemberNames.Token)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaUploadDataMemberNames.Token)]
        public string Token { get; protected set; }
        public PendingMultimediaUpload(int nodeIdRequestingUpload, MultimediaType multimediaType, FileInfo fileInfo, 
            string filePath, string multimediaToken, MultimediaScopeType scopeType, long scopingId, long? scopingId2,
            long? scopingId3, string token, string extension) {
            NodeIdRequestingUpload = nodeIdRequestingUpload;
            MultimediaType = multimediaType;
            FileInfo = fileInfo;
            DirectoryPath = filePath;
            MultimediaToken = multimediaToken;
            ScopeType = scopeType;
            ScopingId = scopingId;
            ScopingId2 = scopingId2;
            ScopingId3 = scopingId3;
            Token = token;
            Extension = extension;
        }
        protected PendingMultimediaUpload() { }
    }
}

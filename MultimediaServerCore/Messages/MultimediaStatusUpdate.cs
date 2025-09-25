using Core.Messages.Messages;
using MultimediaServerCore.DataMemberNames.Messages;
using MultimediaServerCore.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
namespace MultimediaServerCore.Messages
{
    [DataContract]
    public class MultimediaStatusUpdate: TypedMessageBase
    {
        [JsonPropertyName(MultimediaStatusUpdateDataMemberNames.MultimediaType)]
        [JsonInclude]
        [DataMember(Name=MultimediaStatusUpdateDataMemberNames.MultimediaType)]
        public MultimediaType MultimediaType { get; protected set; }
        [JsonPropertyName(MultimediaStatusUpdateDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name=MultimediaStatusUpdateDataMemberNames.MultimediaToken)]
        public string MultimediaToken { get; protected set; }
        [JsonPropertyName(MultimediaStatusUpdateDataMemberNames.Status)]
        [JsonInclude]
        [DataMember(Name = MultimediaStatusUpdateDataMemberNames.Status)]
        public MultimediaItemStatus Status { get; protected set; }
        [JsonPropertyName(MultimediaStatusUpdateDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = MultimediaStatusUpdateDataMemberNames.ScopeType)]
        public MultimediaScopeType ScopeType { get; protected set; }
        [JsonPropertyName(MultimediaStatusUpdateDataMemberNames.ScopingId)]
        [JsonInclude]
        [DataMember(Name = MultimediaStatusUpdateDataMemberNames.ScopingId)]
        public long ScopingId { get; protected set; }
        [JsonPropertyName(MultimediaStatusUpdateDataMemberNames.ScopingId2)]
        [JsonInclude]
        [DataMember(Name = MultimediaStatusUpdateDataMemberNames.ScopingId2)]
        public long? ScopingId2 { get; protected set; }
        [JsonPropertyName(MultimediaStatusUpdateDataMemberNames.ScopingId3)]
        [JsonInclude]
        [DataMember(Name = MultimediaStatusUpdateDataMemberNames.ScopingId3)]
        public long? ScopingId3 { get; protected set; }
        public MultimediaStatusUpdate(
            MultimediaType multimediaType,
            MultimediaScopeType scopeType,
            MultimediaItemStatus status,
            string multimediaToken,
            long scopingId, 
            long? scopingId2,
            long? scopingId3):base()
        {
            MultimediaType = multimediaType;
            MultimediaToken = multimediaToken;
            Status = status;
            ScopeType = scopeType;
            ScopingId = scopingId;
            ScopingId2 = scopingId2;
            ScopingId3 = scopingId3;
            Type = MessageTypes.MultimediaStatusUpdate;
        }
        protected MultimediaStatusUpdate() { }
    }
}

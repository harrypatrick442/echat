using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace MultimediaServerCore
{
    [DataContract]
    public class PendingMultimediaDelete
    {
        [JsonPropertyName(PendingMultimediaDeleteDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaDeleteDataMemberNames.Id)]
        public long Id { get; protected set; }
        [JsonPropertyName(PendingMultimediaDeleteDataMemberNames.FilePath)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaDeleteDataMemberNames.FilePath)]
        public string FilePath { get; protected set; }
        [JsonPropertyName(PendingMultimediaDeleteDataMemberNames.DeletedAt)]
        [JsonInclude]
        [DataMember(Name = PendingMultimediaDeleteDataMemberNames.DeletedAt)]
        public long ScheduledAt { get; protected set; }

        public PendingMultimediaDelete(long id, string filePath, long deletedAt)
            :this(filePath, deletedAt)
        {
            Id = id;
        }
        public PendingMultimediaDelete(string filePath, long deletedAt)
        {
            FilePath = filePath;
            ScheduledAt = deletedAt;
        }
        protected PendingMultimediaDelete() { }
    }
}

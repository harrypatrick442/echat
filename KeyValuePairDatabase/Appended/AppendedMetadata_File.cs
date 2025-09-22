using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KeyValuePairDatabases.Appended
{
    [DataContract]
    public class AppendedMetadata_File
    {
        private int _NFile;
        [JsonPropertyName(AppendedMetadataFileDataMemberNames.CurrentNFile)]
        [JsonInclude]
        [DataMember(Name = AppendedMetadataFileDataMemberNames.CurrentNFile)]
        public int NFile{ get { return _NFile; } protected set { _NFile = value; } }
        private long _Length;
        [JsonPropertyName(AppendedMetadataFileDataMemberNames.Length)]
        [JsonInclude]
        [DataMember(Name = AppendedMetadataFileDataMemberNames.Length)]
        public long Length { get { return _Length; } set { _Length = value; } }
        private long _StartIndexInclusive;
        [JsonPropertyName(AppendedMetadataFileDataMemberNames.StartIndexInclusive)]
        [JsonInclude]
        [DataMember(Name =AppendedMetadataFileDataMemberNames.StartIndexInclusive)]
        public long StartIndexInclusive { get { return _StartIndexInclusive; } protected set { _StartIndexInclusive = value; } }
        public AppendedMetadata_File(int nFile, long startIndexInclusive) {
            _NFile = nFile;
            _StartIndexInclusive = startIndexInclusive;
        }
        public long EndIndexExclusive { get { return Length + StartIndexInclusive; } }
        protected AppendedMetadata_File() { }

    }
}

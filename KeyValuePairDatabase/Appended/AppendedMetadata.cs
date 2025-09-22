
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KeyValuePairDatabases.Appended
{
    [DataContract]
    public class AppendedMetadata
    {
        public AppendedMetadata_File CurrentFile {
            get
            {
                if (_FilesMostRecentFirst == null || _FilesMostRecentFirst.Length < 1)
                    return null;
                return _FilesMostRecentFirst[0];
            }
        }
        private AppendedMetadata_File[] _FilesMostRecentFirst;
        [JsonPropertyName(AppendedMetadataDataMemberNames.Files)]
        [JsonInclude]
        [DataMember(Name = AppendedMetadataDataMemberNames.Files)]
        public AppendedMetadata_File[] FilesMostRecentFirst
        {
            get { return _FilesMostRecentFirst; }
            protected set { _FilesMostRecentFirst = value; }
        }
        public AppendedMetadata_File CreateNextFile(){
            AppendedMetadata_File currentFile = CurrentFile;
            AppendedMetadata_File nextFile = 
                currentFile==null?
                new AppendedMetadata_File(
                    0, 
                    0
                ): new AppendedMetadata_File(
                   currentFile.NFile + 1,
                   currentFile.Length+currentFile.StartIndexInclusive
                );
            AddFile(nextFile);
            return nextFile;
        }
        public void AddFile(AppendedMetadata_File file) {
            if (_FilesMostRecentFirst == null || _FilesMostRecentFirst.Length < 1)
            {
                _FilesMostRecentFirst = new AppendedMetadata_File[] { file };
                return;
            }
            _FilesMostRecentFirst = new AppendedMetadata_File[] { file }
            .Concat(_FilesMostRecentFirst).ToArray();
        }
        public AppendedMetadata_File GetFileContainingStartIndex(long indexToReadFromBackwardsExclusive,
            out int fileIndex) {
            int length;
            fileIndex = 0;
            if (_FilesMostRecentFirst == null || (length= _FilesMostRecentFirst.Length) < 1) return null;
            while (fileIndex < length) {
                AppendedMetadata_File file = _FilesMostRecentFirst[fileIndex++];
                if (indexToReadFromBackwardsExclusive > file.StartIndexInclusive) {
                    return file;
                }
            }
            return null;
        }
        public AppendedMetadata_File GetFileAtIndex(int index) {
            return _FilesMostRecentFirst?.Skip(index).FirstOrDefault();
        }
        public AppendedMetadata() { }
    }
}

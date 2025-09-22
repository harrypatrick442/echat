namespace FileServerBase
{
    public class DynamicCachedFile : IDynamicCachedFile
    {
        private string _ContentType;
        private byte[] _Bytes;
        public string RequestPath { get; }
        public bool IsIndex { get; }
        private string _FilePath, _RequestPath;
        public DynamicCachedFile(string filePath, string requestPath, bool isIndex)
        {
            _FilePath = filePath;
            _RequestPath = requestPath;
            IsIndex = isIndex;
            _Bytes = File.ReadAllBytes(filePath);
            _ContentType = MimeTypes.MimeTypeMap.GetMimeType(filePath);
            RequestPath = requestPath;
        }
        public byte[] GetBytes(out string contentType)
        {
            contentType = _ContentType;
            return _Bytes;
        }
        public void Dispose()
        {
        }
    }
}
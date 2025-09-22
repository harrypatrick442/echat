namespace MultimediaServerCore
{
    public sealed class MultimediaCacheEntry
    {
        public byte[] Bytes { get; }
        public string Path { get; }
        public string ContentType { get; }
        public long Size { get { return Bytes.Length; } }
        public MultimediaCacheEntry(string path, byte[] bytes, 
            string contentType) {
            Path = path;
            Bytes = bytes;
            ContentType = contentType;
        }
    }
}
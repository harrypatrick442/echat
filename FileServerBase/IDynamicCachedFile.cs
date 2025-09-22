namespace FileServerBase
{
    public interface IDynamicCachedFile
    {
        bool IsIndex { get; }
        string RequestPath { get; }

        void Dispose();
        byte[] GetBytes(out string contentType);
    }
}
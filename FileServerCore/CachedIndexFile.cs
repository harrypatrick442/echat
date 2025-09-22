namespace FileServerCore
{
    public class CachedIndexFile:CachedFile
    {
        public CachedIndexFile(string directoryPath)
            :base(Path.Combine(directoryPath, "index.html"))
        {

        }
    }
}

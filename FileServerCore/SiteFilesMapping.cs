namespace FileServerCore
{
    public class SiteFilesMapping
    {
        public string Domain { get; }
        public string DirectoryPath { get; }
        public int IndexExpireMilliseconds { get; }
        public SiteFilesMapping(string domain, string directoryPath, int indexExpireMilliseconds) {
            Domain = domain;
            DirectoryPath = directoryPath;
            IndexExpireMilliseconds = indexExpireMilliseconds;
        }
    }
}

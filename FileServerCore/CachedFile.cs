using ConfigurationCore;
using Core.Timing;
using DependencyManagement;
using Logging;

namespace FileServerCore
{
    public class CachedFile
    {
        private string _Path;
        public CachedFile(string path)
        {
            _Path = path;

        }
        private long _CachedIndexHtmlContentExpiresAtMillisecondsUTC = 0;
        private string _CachedIndexHtmlContent;
        public string Content
        {
            get
            {
                lock (this)
                {
                    long now = TimeHelper.MillisecondsNow;
                    if (now >= _CachedIndexHtmlContentExpiresAtMillisecondsUTC)
                    {
                        try
                        {
                            _CachedIndexHtmlContent = System.IO.
                                File.ReadAllText(_Path);
                            _CachedIndexHtmlContentExpiresAtMillisecondsUTC = now
                                + DependencyManager.Get<ITimeoutsConfiguration>().CachedIndexHtmlExpiresAfterMilliseconds;
                        }
                        catch (Exception ex)
                        {
                            Logs.Default.Error(ex);
                        }
                    }
                    return _CachedIndexHtmlContent;
                }
            }
        }
    }
}

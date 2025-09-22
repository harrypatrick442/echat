using Logging;
using Shutdown;
using FileServerBase;
namespace FileServer
{
    public class DynamicFileServer
    {
        private readonly Dictionary<string, DynamicCachedFilesHost> _MapHostToCachedFilesForHost
            = new Dictionary<string, DynamicCachedFilesHost>();
        private readonly FileSystemWatcher _FileSystemWatcher;
        private DelegateProvideDynamicCachedFile _ProvideDynamicCachedFile;
        public DynamicFileServer(string directoryPath, 
            DelegateProvideDynamicCachedFile provideDynamicCachedFile = null)
        {
            _ProvideDynamicCachedFile = provideDynamicCachedFile;
            MapExistingDirectories(directoryPath);
            _FileSystemWatcher = new FileSystemWatcher(directoryPath);
            _FileSystemWatcher.Created += HandleCreated;
            _FileSystemWatcher.Error += HandleError;
            _FileSystemWatcher.Deleted += HandleDeleted;
            _FileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            _FileSystemWatcher.EnableRaisingEvents = true;
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.FileServer);
        }
        private void MapExistingDirectories(string directoryPath) {
            foreach (string childDirectory in Directory.GetDirectories(directoryPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(childDirectory);
                _MapHostToCachedFilesForHost.Add(directoryInfo.Name, 
                    new DynamicCachedFilesHost(childDirectory, directoryInfo.Name, _ProvideDynamicCachedFile));
            }
        }
        private void HandleCreated(object sender, FileSystemEventArgs e)
        {
            var fileHost = new DynamicCachedFilesHost(e.FullPath, e.Name, _ProvideDynamicCachedFile);
            lock (_MapHostToCachedFilesForHost)
            {
                _MapHostToCachedFilesForHost.Add(e.Name, fileHost);
            }
        }
        private void HandleDeleted(object sender, FileSystemEventArgs e)
        {
            lock (_MapHostToCachedFilesForHost)
            {
                if (_MapHostToCachedFilesForHost.TryGetValue(e.Name, out DynamicCachedFilesHost cachedFilesForHost))
                {
                    cachedFilesForHost.Dispose();
                    _MapHostToCachedFilesForHost.Remove(e.Name);
                }
            }
        }
        private void HandleError(object sender, ErrorEventArgs e)
        {
            Logs.Default.Error(e.GetException());
        }
        public DynamicCachedFilesHost GetCachedFilesForHost(string host)
        {
            lock (_MapHostToCachedFilesForHost)
            {
                _MapHostToCachedFilesForHost.TryGetValue(host, out DynamicCachedFilesHost cachedFilesForHost);
                return cachedFilesForHost;
            }
        }
        public void Dispose()
        {
            _FileSystemWatcher.Dispose();
            lock (_MapHostToCachedFilesForHost)
            {
                foreach (DynamicCachedFilesHost cachedFilesForHost in _MapHostToCachedFilesForHost.Values)
                    cachedFilesForHost.Dispose();
                _MapHostToCachedFilesForHost.Clear();
            }
        }
    }
}
using Logging;
using Core.FileSystem;

namespace FileServerBase
{
    public class DynamicCachedFilesHost
    {
        private Dictionary<string, IDynamicCachedFile> _MapPathToCachedFile 
            = new Dictionary<string, IDynamicCachedFile>();
        private DelegateProvideDynamicCachedFile _ProvideDynamicCachedFile;
        private readonly FileSystemWatcher _FileSystemWatcher, _FileSystemWatcherChildDiretories;
        private int _IndexFilePathStartsFrom;
        public string Host { get; }
        public DynamicCachedFilesHost(string directoryPath, string host, DelegateProvideDynamicCachedFile provideDynamicCachedFile)
        {
            Host = host;
            _IndexFilePathStartsFrom = directoryPath.Length;
            _ProvideDynamicCachedFile = provideDynamicCachedFile;   
            CacheAllFiles(directoryPath);
            _FileSystemWatcher = new FileSystemWatcher(directoryPath)
            {
                IncludeSubdirectories = true
            };
            _FileSystemWatcher.Created += HandleCreated;
            _FileSystemWatcher.Error += HandleError;
            _FileSystemWatcher.Changed += HandleChanged;
            _FileSystemWatcher.Deleted += HandleDeleted;
            _FileSystemWatcher.NotifyFilter = NotifyFilters.FileName|NotifyFilters.LastWrite;
            _FileSystemWatcher.EnableRaisingEvents = true;

            _FileSystemWatcherChildDiretories = new FileSystemWatcher(directoryPath)
            {
                IncludeSubdirectories = true
            };
            _FileSystemWatcherChildDiretories.Deleted += HandleDirectoryDeleted;
            _FileSystemWatcherChildDiretories.NotifyFilter = NotifyFilters.DirectoryName;
            _FileSystemWatcherChildDiretories.EnableRaisingEvents = true;

        }
        private void HandleChanged(object sender, FileSystemEventArgs e)
        {
            string requestPath = GetRequestPathFromFullFilePath(e.FullPath);
            IDynamicCachedFile cachedFile;
            try
            {
                if (!File.Exists(e.FullPath)) return;
                cachedFile = NewDynamicCachedFile(e.FullPath, requestPath);
            }
            catch (IOException ex) {
                return;
            }
            lock (_MapPathToCachedFile)
            {
                if(_MapPathToCachedFile.TryGetValue(requestPath, out IDynamicCachedFile existingCachedFile))
                    existingCachedFile.Dispose();
                _MapPathToCachedFile[requestPath] = cachedFile;
                if (cachedFile.IsIndex)
                    _MapPathToCachedFile["/"] = cachedFile;
            }
        }
        private IDynamicCachedFile NewDynamicCachedFile(string filePath, string requestPath) {
            bool isIndex = Path.GetFileName(filePath) == "index.html";
            IDynamicCachedFile dynamicCachedFile;
            if (_ProvideDynamicCachedFile != null)
            {
                dynamicCachedFile = _ProvideDynamicCachedFile(filePath, requestPath, isIndex, this);
                if (dynamicCachedFile != null) 
                    return dynamicCachedFile;
            }
            return new DynamicCachedFile(filePath, requestPath, isIndex);
        }
        private void HandleCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                CacheFile(e.FullPath);
            }
            catch(IOException ex)
            {

            }
        }
        private void HandleDeleted(object sender, FileSystemEventArgs e)
        {
                string requestPath = GetRequestPathFromFullFilePath(e.FullPath);
            lock (_MapPathToCachedFile)
            {
                if (_MapPathToCachedFile.TryGetValue(requestPath, out IDynamicCachedFile cachedFile))
                {
                    _MapPathToCachedFile.Remove(requestPath);
                    if (cachedFile != null)
                    {
                        cachedFile.Dispose();
                        if (cachedFile.IsIndex)
                            _MapPathToCachedFile.Remove("/");
                    }
                }
            }
        }
        private void HandleDirectoryDeleted(object sender, FileSystemEventArgs e)
        {
            string requestPath = GetRequestPathFromFullFilePath(e.FullPath);
            lock (_MapPathToCachedFile)
            {
                foreach (DynamicCachedFile cachedFile in _MapPathToCachedFile.Values) {
                    if (cachedFile.RequestPath.IndexOf(requestPath) == 0) {
                        _MapPathToCachedFile.Remove(cachedFile.RequestPath);
                        if (cachedFile.IsIndex)
                            _MapPathToCachedFile.Remove("/");
                        cachedFile.Dispose();
                    }
                }
            }
        }
        private void HandleError(object sender, ErrorEventArgs e)
        {
            Logs.Default.Error(e.GetException());
        }
        private void CacheAllFiles(string directoryPath) {
            FileInfo[] fileInfos = DirectoryHelper.GetFilesRecursively(new DirectoryInfo(directoryPath));
            foreach (FileInfo fileInfo in fileInfos)
            {
                CacheFile(fileInfo.FullName);
            }
        }
        private void CacheFile(string filePath) {
            string requestPath = GetRequestPathFromFullFilePath(filePath);
            IDynamicCachedFile cachedFile = NewDynamicCachedFile(filePath, requestPath);
            lock (_MapPathToCachedFile)
            {
                _MapPathToCachedFile[requestPath] = cachedFile;
                if (cachedFile.IsIndex)
                    _MapPathToCachedFile["/"] = cachedFile;
            }
        }
        private string GetRequestPathFromFullFilePath(string filePath) {
            string relativeFilePath = filePath.Substring(_IndexFilePathStartsFrom);
            return relativeFilePath.Replace('\\', '/').Replace("..", "");
        }
        public byte[] GetBytes(string path, out string contentType)
        {
            IDynamicCachedFile cachedFile;
            lock (_MapPathToCachedFile)
            {
                if (!_MapPathToCachedFile.TryGetValue(path, out cachedFile))
                {
                    contentType = null;
                    return null;
                }
            }
            return cachedFile.GetBytes(out contentType);
        }
        public IDynamicCachedFile GetCachedFile(string path)
        {
            IDynamicCachedFile cachedFile;
            lock (_MapPathToCachedFile)
            {
                if (!_MapPathToCachedFile.TryGetValue(path, out cachedFile))
                {
                    return null;
                }
            }
            return cachedFile;
        }
        public void Dispose() {
            _FileSystemWatcher.Dispose(); 
            _FileSystemWatcherChildDiretories.Dispose(); 
        }
    }
}
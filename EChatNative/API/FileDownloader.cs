
using Core.Exceptions;

namespace EChatNative.API
{
    public class FileDownloader
    {
        private static FileDownloader? _Instance;
        private string _DownloadsDirectoryPath;
        public static FileDownloader Initializer(string downloadsDirectoryPath) {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(FileDownloader));
            _Instance = new FileDownloader(downloadsDirectoryPath);
            return _Instance;
        }
        public static FileDownloader Instance
        {
            get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(FileDownloader));
                return _Instance;
            }
        }
        private FileDownloader(string downloadsDirectoryPath) {
            _DownloadsDirectoryPath = downloadsDirectoryPath;
        }
        public Action DownloadFile(string url, string fileName, out string directoryPath)
        {
            string filePath = _GetFilePath(fileName, out directoryPath);
            return () =>
            {
                using (Stream fileStream = System.IO.File.OpenWrite(filePath))
                {
                    HttpClient httpClient = new HttpClient();
                    httpClient.Timeout = Timeout.InfiniteTimeSpan;
                    HttpRequestMessage request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get,
                    };
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    Task<HttpResponseMessage> task = httpClient.SendAsync(request, cancellationTokenSource.Token);
                    task.Wait();
                    HttpResponseMessage httpResponseMessage = task.Result;
                    httpResponseMessage.Content.ReadAsStream().CopyToAsync(fileStream)
                        .Wait();
                    fileStream.Flush();
                    fileStream.Close();
                }
            };
        }
        private string _GetFilePath(string fileName, out string directoryPath)
        {
            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string currentFileName = $"{fileNameWithoutExtension}{extension}";
            directoryPath = _DownloadsDirectoryPath;
            int n = 0;
            while (true)
            {
                string filePath = Path.Combine(directoryPath, currentFileName);
                if (!File.Exists(filePath))
                    return filePath;
                currentFileName = $"{fileNameWithoutExtension}({++n}){extension}";
            }
        }
    }
}
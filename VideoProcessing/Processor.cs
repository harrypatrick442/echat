using Core.Exceptions;
using Core.FileSystem;
using Core.Processes;
using FFmpeg.AutoGen;
using System.Text;
namespace VideoProcessing
{
    public class Processor
    {
        private static Processor? _Instance;
        public static Processor Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(Processor));
            _Instance = new Processor();
            return _Instance!;
        }
        public static Processor Instance { 
            get 
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(Processor));
                return _Instance;
            } 
        }
        private TemporaryFile _FfmpegTemporaryFile;
        private string _FfmpegFilePath, _FfmpegDirectoryPath;
        private Processor() {
            _FfmpegTemporaryFile = Ffmpeg.Unpackage(out string filePath, out string directoryPath);
            _FfmpegFilePath = filePath;
            _FfmpegDirectoryPath = directoryPath!;
        }
        public TemporaryFile ConvertToTemporaryFile(string fromFilePath, string toExtension)
        {
            TemporaryFile temporaryFileResult = new TemporaryFile(toExtension);
            Convert(fromFilePath, temporaryFileResult.FilePath);
            return temporaryFileResult;
        }
        public void Convert(string fromFilePath, string toFilePath) {
            string arguments = $"-hide_banner -loglevel error -i \"{fromFilePath}\" \"{toFilePath}\"";
            StringBuilder sbError = new StringBuilder();
            using (RunningProcessHandle handle = ProcessRunHelper.RunAsynchronously(
                _FfmpegFilePath,
                _FfmpegDirectoryPath,
                arguments,
                (str) => { },
                (str) => sbError.Append(str)))
            {
                handle.Wait();
                if (sbError.Length > 0)
                    throw new Exception(sbError.ToString());
            }
        }
        public void ExtractThumbnail(string videoFilePath, string thumbnailFilePath)
        { 
            


            string arguments = $"-hide_banner -loglevel error -i \"{videoFilePath}\" -ss 00:00:01 -vframes 1 \"{thumbnailFilePath}\"";
            StringBuilder sbError = new StringBuilder();
            using (RunningProcessHandle handle = ProcessRunHelper.RunAsynchronously(
                _FfmpegFilePath,
                _FfmpegDirectoryPath,
                arguments,
                (str) => { },
                (str) => sbError.Append(str)))
            {
                handle.Wait();
                if (sbError.Length > 0)
                    throw new Exception(sbError.ToString());
            }
        }
    }
}

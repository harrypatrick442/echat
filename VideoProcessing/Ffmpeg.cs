using Core.Enums;
using Core.FileSystem;
using System.Runtime.InteropServices;

namespace VideoProcessing
{
    public static class Ffmpeg
    {
        public static TemporaryFile Unpackage(out string filePath, out string directoryPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                TemporaryFile temporaryFile = new TemporaryFile(".exe");
                File.WriteAllBytes(temporaryFile.FilePath, Resource1.ffmpegexe);
                filePath = temporaryFile.FilePath;
                directoryPath = Path.GetDirectoryName(filePath)!;
                return temporaryFile;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePath = "ffmpeg";
                directoryPath = null;
                return null;
            }
            throw new NotImplementedException();
        }
    }
}

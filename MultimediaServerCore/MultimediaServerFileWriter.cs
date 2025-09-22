using Core.Exceptions;
using Shutdown;
namespace MultimediaServerCore
{
    public static class MultimediaServerFileWriter
    {
        public static void Write(PendingMultimediaUpload multimediaUpload, Stream stream) {
            using (var fileStream = File.Create(multimediaUpload.DirectoryPath))
            {
                stream.CopyTo(fileStream);
            }
        }
    }
}
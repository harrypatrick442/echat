using Logging;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Messages;
using Snippets.Core.ImageProcessing;
using System.Diagnostics;
namespace MultimediaServerCore
{
    public static class UploadedMultimediaProcessor
    {
        public static void ProcessMultimedia(Stream stream, 
            PendingMultimediaUpload pendingMultimediaUpload)
        {
            Action<Exception?> done = Get_Done(pendingMultimediaUpload);
            try
            {
                switch (pendingMultimediaUpload.MultimediaType)
                {
                    case MultimediaType.ProfilePicture:
                        ProcessProfilePicture(stream, pendingMultimediaUpload, done);
                        break;
                    case MultimediaType.ConversationPicture:
                        ProcessChatRoomPicture(stream, pendingMultimediaUpload, done);
                        break;
                    case MultimediaType.MessagePicture:
                        ProcessMessagePicture(stream, pendingMultimediaUpload, done);
                        break;
                    case MultimediaType.MessageVideo:
                        ProcessMessageVideo(stream, pendingMultimediaUpload, done);
                        break;
                    case MultimediaType.CustomEmoticon:

                        break;
                }
            }
            catch(Exception ex) { 
                done(ex);
            }
            finally
            {
                stream.Dispose();
            }
        }
        private static Action<Exception?> Get_Done(
            PendingMultimediaUpload pendingMultimediaUpload)
        {
            return (ex) =>
            {
                MultimediaStatusUpdate multimediaUploadUpdate;
                if (ex == null)
                {
                    multimediaUploadUpdate = new MultimediaStatusUpdate(
                        pendingMultimediaUpload.MultimediaType, pendingMultimediaUpload.ScopeType,
                        MultimediaItemStatus.Live, pendingMultimediaUpload.MultimediaToken, 
                        pendingMultimediaUpload.ScopingId, pendingMultimediaUpload.ScopingId2,
                        pendingMultimediaUpload.ScopingId3);
                }
                else
                {
                    Logs.Default.Error(ex);
                    multimediaUploadUpdate = new MultimediaStatusUpdate(
                            pendingMultimediaUpload.MultimediaType, pendingMultimediaUpload.ScopeType,
                            MultimediaItemStatus.Failed, pendingMultimediaUpload.MultimediaToken, pendingMultimediaUpload.ScopingId,
                            pendingMultimediaUpload.ScopingId2, pendingMultimediaUpload.ScopingId3);
                }
                try
                {
                    MultimediaServerMesh.Instance.MultimediaUploadUpdate(
                        pendingMultimediaUpload.NodeIdRequestingUpload, multimediaUploadUpdate
                    );
                }
                catch (Exception ex2)
                {
                    Logs.Default.Error(ex2);
                }
            };
        }
        private static void ProcessProfilePicture(Stream stream, PendingMultimediaUpload pendingMultimediaUpload, Action<Exception?> done)
        {
            string fileName = pendingMultimediaUpload.Token + pendingMultimediaUpload.Extension;
            string directoryPath = pendingMultimediaUpload.DirectoryPath;
                string uploadFilePath = WriteFile(stream, directoryPath, fileName);
            new Thread(() =>
            {
                try
                {
                    string thumbnailImagePath = Path.Combine(directoryPath, MultimediaTokenHelper.GetThumbnailFileName(pendingMultimediaUpload.Token, pendingMultimediaUpload.Extension));
                    ImageProcessing.ResizeImageToFile(uploadFilePath, 128, 128, thumbnailImagePath);
                    string microRoundedThumbnailImagePath = Path.Combine(directoryPath, MultimediaTokenHelper.GetMicroRoundedThumbnailFileName(pendingMultimediaUpload.Token, pendingMultimediaUpload.Extension));
                    ImageProcessing.ClipToCircle(thumbnailImagePath, 32, microRoundedThumbnailImagePath);
                        done(null);
                }
                catch (Exception ex) {
                    done(ex);
                }
            }).Start();
        }
        private static void ProcessChatRoomPicture(Stream stream, PendingMultimediaUpload pendingMultimediaUpload, Action<Exception?> done)
        {
            string fileName = pendingMultimediaUpload.Token + pendingMultimediaUpload.Extension;
            string directoryPath = pendingMultimediaUpload.DirectoryPath;
            string uploadFilePath = WriteFile(stream, directoryPath, fileName);
            new Thread(() =>
            {
                try
                {
                    string thumbnailImagePath = Path.Combine(directoryPath, MultimediaTokenHelper.GetThumbnailFileName(pendingMultimediaUpload.Token, pendingMultimediaUpload.Extension));
                    ImageProcessing.ResizeImageToFile(uploadFilePath, 128, 128, thumbnailImagePath);
                    done(null);
                }
                catch (Exception ex)
                {
                    done(ex);
                }
            }).Start();
        }
        private static void ProcessMessagePicture(Stream stream, PendingMultimediaUpload pendingMultimediaUpload, Action<Exception?> done)
        {
            string fileName = pendingMultimediaUpload.Token + pendingMultimediaUpload.Extension;
            string directoryPath = pendingMultimediaUpload.DirectoryPath;
            WriteFile(stream, directoryPath, fileName);
            done(null);
        }
        private static void ProcessMessageVideo(Stream stream, 
            PendingMultimediaUpload pendingMultimediaUpload, Action<Exception?> done)
        {
            string extension = pendingMultimediaUpload.Extension;
            string fileName = pendingMultimediaUpload.Token + extension;
            string directoryPath = pendingMultimediaUpload.DirectoryPath;
            Directory.CreateDirectory(directoryPath);
            string uploadedFilePath = WriteFile(stream, directoryPath, fileName);
            Logs.Default.Info("uploadedFilePath is: " + uploadedFilePath);
            new Thread(() =>
            {
                try
                {
                    string thumbnailImagePath = Path.Combine(directoryPath, MultimediaTokenHelper.GetThumbnailFileName(pendingMultimediaUpload.Token, ".webp"));
                    VideoProcessing.Processor.Instance.ExtractThumbnail(
                        uploadedFilePath,
                        thumbnailImagePath
                    );
                    done(null);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                    done(null);
                }
            }).Start();
            /*
            VideoProcessing.Processor.Instance.Convert(
                temporaryFile.FilePath, 
                filePathToWriteTo
            );*/
        }
        private static string WriteFile(Stream stream, string directoryPath, string fileName) {

            Directory.CreateDirectory(directoryPath);
            string filePathToWriteTo = Path.Combine(directoryPath, fileName);
            using (var fileStream = System.IO.File.Create(
            filePathToWriteTo))
            {
                stream.CopyTo(fileStream);
            }
            return filePathToWriteTo;
        }
    }
}
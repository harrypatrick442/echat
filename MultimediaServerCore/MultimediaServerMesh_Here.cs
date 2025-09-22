using Logging;
using MultimediaServerCore.Enums;
using MultimediaServerCore.Messages;
using FileInfo = Core.Messages.Messages.FileInfo;

namespace MultimediaServerCore
{
    public partial class MultimediaServerMesh
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="failedReason"></param>
        /// <returns>url to post file to</returns>
        public void PrepareToUpload_Here(int nodeIdRequestingUpload,
            MultimediaType multimediaType, FileInfo fileInfo, MultimediaScopeType scopeType,
            long scopingId, long? scopingId2, long? scopingId3, out MultimediaFailedReason? failedReason,
            out string multimediaToken)
        {
            try
            {
                PendingMultimediaUploads.Instance.PrepareToUpload(nodeIdRequestingUpload, multimediaType, fileInfo, 
                    scopeType, scopingId, scopingId2, scopingId3, out failedReason, out multimediaToken);
            }
            catch (Exception ex) {

                failedReason = MultimediaFailedReason.ServerError;
                multimediaToken = null;
                Logs.Default.Error(ex);
            }
        }
        private void MultimediaUploadUpdate_Here(
            MultimediaStatusUpdate successfulMultimediaUpload)
        {
            try
            {
                MultimediaUploadsEventSource.DispatchStatusUpdate(successfulMultimediaUpload);
            }
            catch (Exception ex) 
            {
                Logs.Default.Error(ex);
            }
        }
        public void Delete_Here(string[] rawMultimediaTokens) {
            foreach(string rawMultimediaToken in rawMultimediaTokens)
                MultimediaDeletesProcessor.Instance.Add(MultimediaToken.FromString(rawMultimediaToken));
        }
    }
}
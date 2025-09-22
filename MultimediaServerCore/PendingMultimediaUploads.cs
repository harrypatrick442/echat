using Core.Exceptions;
using MultimediaServerCore.Enums;
using FileInfo = Core.Messages.Messages.FileInfo;
namespace MultimediaServerCore
{
    public sealed class PendingMultimediaUploads
    {
        private static PendingMultimediaUploads _Instance;
        public static PendingMultimediaUploads Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(PendingMultimediaUploads));
            _Instance = new PendingMultimediaUploads();
            return _Instance;
        }
        public static PendingMultimediaUploads Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(PendingMultimediaUploads));
                return _Instance;
            }
        }
        private Dictionary<string, PendingMultimediaUpload> _MapTokenToPendingMultimediaUploadMetadata_Oldest = new Dictionary<string, PendingMultimediaUpload>();
        private Dictionary<string, PendingMultimediaUpload> _MapTokenToPendingMultimediaUploadMetadata_Latest = new Dictionary<string, PendingMultimediaUpload>();
        private static readonly object _LockObjectDateDirectory = new object();
        private static string _CurrentDateDirectories;
        private static long _StartOfNextDay;
        private static long MILLISECONDS_IN_A_DAY = 3600 * 24;
        private PendingMultimediaUploads() {

        }
        public int Count
        {
            get
            {
                lock (_MapTokenToPendingMultimediaUploadMetadata_Latest)
                {
                    return _MapTokenToPendingMultimediaUploadMetadata_Latest.Count 
                        + _MapTokenToPendingMultimediaUploadMetadata_Oldest.Count;
                }
            }
        }
        public PendingMultimediaUpload? Take(string token) {
            lock (_MapTokenToPendingMultimediaUploadMetadata_Latest)
            {
                if (_MapTokenToPendingMultimediaUploadMetadata_Latest.TryGetValue(token, out PendingMultimediaUpload? metadata))
                {
                    _MapTokenToPendingMultimediaUploadMetadata_Latest.Remove(token);
                    return metadata;
                }
                if (_MapTokenToPendingMultimediaUploadMetadata_Oldest.TryGetValue(token, out metadata))
                {
                    _MapTokenToPendingMultimediaUploadMetadata_Oldest.Remove(token);
                    return metadata;
                }
                return null;
            }
        }
        public void PrepareToUpload(int nodeIdRequestingUpload, MultimediaType multimediaType,
            FileInfo fileInfo, MultimediaScopeType scopeType, long scopingId, long? scopingId2, 
            long? scopingId3, out MultimediaFailedReason? failedReason, out string multimediaToken)
        {
            multimediaToken = null;
            string extension = Path.GetExtension(fileInfo.Name);
            failedReason = MultimediaUploadValidation.Instance.Validate(
                multimediaType, extension, fileInfo);
            if (failedReason != null) 
                return;
            string token;
            lock(_MapTokenToPendingMultimediaUploadMetadata_Latest)
            {
                token = NextToken();
            }
            multimediaToken = MultimediaTokenHelper.GetMultimediaTokenAndFilePath(multimediaType, extension, token,
                out string filePath);
            PendingMultimediaUpload pendingMultimediaUpload = new PendingMultimediaUpload(
                nodeIdRequestingUpload, multimediaType, fileInfo,
                filePath, multimediaToken, scopeType, scopingId,
                scopingId2, scopingId3, token, extension);

            lock (_MapTokenToPendingMultimediaUploadMetadata_Latest)
            {
                _MapTokenToPendingMultimediaUploadMetadata_Latest[token] = pendingMultimediaUpload;
            }
        }
        private string NextToken() {
            while (true) { 
                string token = Guid.NewGuid().ToString("N");
                if (_MapTokenToPendingMultimediaUploadMetadata_Latest.ContainsKey(token))
                    continue;
                if (_MapTokenToPendingMultimediaUploadMetadata_Oldest.ContainsKey(token))
                    continue;
                return token;
            }
        }
    }
}
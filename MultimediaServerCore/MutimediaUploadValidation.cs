using Core.Exceptions;
using Microsoft.VisualBasic;
using MultimediaServerCore.Enums;
using FileInfo = Core.Messages.Messages.FileInfo;

namespace MultimediaServerCore
{
    public class MultimediaUploadValidation
    {
        private static MultimediaUploadValidation _Instance;
        public static MultimediaUploadValidation Initialize(MultimediaServerSetup setup)
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MultimediaUploadValidation));
            _Instance = new MultimediaUploadValidation(setup);
            return _Instance;
        }
        public static MultimediaUploadValidation Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(MultimediaUploadValidation));
                return _Instance;
            }
        }
        private Dictionary<MultimediaType, HashSet<string>> _MapMultimediaTypeToAllowedFileExtensions
            = new Dictionary<MultimediaType, HashSet<string>>();
        private MultimediaUploadValidation(MultimediaServerSetup setup) {
            foreach (MultimediaTypeSetup multimediaTypeSetup in setup.MultimediaTypeSetups) {
                _MapMultimediaTypeToAllowedFileExtensions[multimediaTypeSetup.MultimediaType]
                    = new HashSet<string>(multimediaTypeSetup.AllowedExtensions);
            }
        }
        public MultimediaFailedReason? Validate(
            MultimediaType multimediaType, string fileExtension, FileInfo fileInfo)
        {
            if (fileInfo.Size > GlobalConstants.Sizes.MULTIMEDIA_SERVER_MAXIMUM_FILE_SIZE)
            {
                return MultimediaFailedReason.FileTooLarge;
            }
            if (string.IsNullOrEmpty(fileExtension))
            {
                return MultimediaFailedReason.FileTypeNotSupported;
            }
            if (!FileTypeSupported(multimediaType, fileExtension))
            {
                return MultimediaFailedReason.FileTypeNotSupported;
            }
            return null;
        }
        private bool FileTypeSupported(MultimediaType multimediaType, string extension)
        {
            if (!_MapMultimediaTypeToAllowedFileExtensions.TryGetValue(multimediaType, out HashSet<string> allowedFileExtensions)) {
                return false;
            }
            return allowedFileExtensions.Contains(extension);
        }
    }
}
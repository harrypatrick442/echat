using MultimediaServerCore.Enums;
using FileInfo = Core.Messages.Messages.FileInfo;

namespace MultimediaServerCore
{
    public class MultimediaTypeSetup
    {
        public MultimediaType MultimediaType { get; } 
        public string[] AllowedExtensions { get; }
        public MultimediaTypeSetup(MultimediaType multimediaType, params string[] allowedExtensions) { 
            MultimediaType = multimediaType;
            AllowedExtensions = allowedExtensions;
        }
    }
}
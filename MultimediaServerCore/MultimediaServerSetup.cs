using MultimediaServerCore.Enums;
using FileInfo = Core.Messages.Messages.FileInfo;

namespace MultimediaServerCore
{
    public class MultimediaServerSetup
    {
        public MultimediaTypeSetup[] MultimediaTypeSetups { get; }
        public MultimediaServerSetup(params MultimediaTypeSetup[] multimediaTypeSetups) { 
            MultimediaTypeSetups = multimediaTypeSetups;
        }
    }
}
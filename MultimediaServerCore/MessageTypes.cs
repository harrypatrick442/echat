using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace MultimediaServerCore
{
    public class MessageTypes
    {
        public const string
            //MultimediaPrepareToUpload = InterserverMessageTypes.MultimediaPrepareToUpload,
            MultimediaStatusUpdate = InterserverMessageTypes.MultimediaUploadUpdate,
            MultimediaUploadFailed = "muf",
            MultimediaMetadataUpdate = "ummu",
            MultimediaDelete = "umd",
            MultimediaDeletePending = "mdp";
    }
}
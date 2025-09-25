using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace UserMultimediaCore
{
    public class MessageTypes
    {
        public const string
            MultimediaUploadProfilePicture = "mupp",
            MultimediaUpdateProfilePictureMetadata = InterserverMessageTypes.MultimediaUpdateProfilePictureMetadata,
            MultimediaDeleteProfilePicture = InterserverMessageTypes.MultimediaDeleteProfilePicture;
    }
}
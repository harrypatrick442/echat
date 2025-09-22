using Core.DataMemberNames.Messages;
using MessageTypes.Attributes;
namespace UserMultimediaCore.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.MultimediaUploadProfilePicture)]
    public class UploadProfilePictureRequestDataMemberNames
    {
        [DataMemberNamesClass(typeof(FileInfoDataMemberNames), isArray: false)]
        public const string FileInfo = "f";
        public const string XRating = "x";
        public const string VisibleTo = "v";
        public const string Description = "d";
        public const string SetAsMain = "m";
    }
}
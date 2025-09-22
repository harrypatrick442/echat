using Core.DataMemberNames.Messages;
using MessageTypes.Attributes;
namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatUploadMessagePicture)]
    public class UploadMessagePictureRequestDataMemberNames
    {
        [DataMemberNamesClass(typeof(FileInfoDataMemberNames), isArray: false)]
        public const string FileInfo = "f";
        public const string XRating = "x";
        public const string VisibleTo = "v";
        public const string Description = "d";
    }
}
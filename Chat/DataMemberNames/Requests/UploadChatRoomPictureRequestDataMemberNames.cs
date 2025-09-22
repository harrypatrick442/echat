using MessageTypes.Attributes;
using Core.DataMemberNames.Messages;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatUploadRoomPicture)]
    public static class UploadChatRoomPictureRequestDataMemberNames
    {
        [DataMemberNamesClass(typeof(FileInfoDataMemberNames))]
        public const string FileInfo = "f";
        public const string
            XRating = "x",
            Description = "d",
            VisibleTo = "v";
    }
}
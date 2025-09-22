using Core.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatMultimediaUpload)]
    public static class ChatMultimediaUploadRequestDataMemberNames
    {
        public const string
            MultimediaType = "m",
            ConversationId = "c",
            ConversationType = "t";
        [DataMemberNamesClass(typeof(FileInfoDataMemberNames))]
        public const string
            FileInfo = "f";

        public const string
            UserId = "u",
            SessionId = "r",
            XRating = "x",
            Description = "d",
            AlreadyCheckedPermission = "a",
            ScopeType = "s";
    }
}
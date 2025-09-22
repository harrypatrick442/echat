using MessageTypes.Attributes;
using MultimediaCore.DataMemberNames.Messages;

namespace Chat.DataMemberNames.Responses
{
    public static class ChatMultimediaUploadResponseDataMemberNames
    {
        [DataMemberNamesClass(typeof(UserMultimediaItemDataMemberNames))]
        public const string UserMultimediaItem = "u";
        public const string FailedReason = "f";
    }
}
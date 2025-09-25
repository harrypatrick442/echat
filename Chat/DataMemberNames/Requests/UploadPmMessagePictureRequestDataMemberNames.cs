using MessageTypes.Attributes;
namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatPmUploadMessagePicture)]
    public class UploadPmMessagePictureRequestDataMemberNames : UploadMessagePictureRequestDataMemberNames
    {
        public const string ConversationId = "c";
        public const string ConversationType = "t";
    }
}
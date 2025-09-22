using MessageTypes.Attributes;
namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ChatPmUploadMessageVideo)]
    public class UploadPmMessageVideoRequestDataMemberNames : UploadMessageVideoRequestDataMemberNames
    {
        public const string ConversationId = "c";
        public const string ConversationType = "t";
    }
}
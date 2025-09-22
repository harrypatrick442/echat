using Chat.DataMemberNames.Messages;
using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Responses
{
    public static class SendMessageResponseDataMemberNames
    {
        public const string
            Successful = "s",
            FailedReason = "f";
        //[DataMemberNamesClass(typeof(global::MessageTypes.MessageTypes))]
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames))]
        public const string ReplyMessage = "r";
    }
}
using MessageTypes.Attributes;
using Chat.DataMemberNames.Messages;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ChatRoomSendMessage)]
    public static class SendChatRoomMessageRequestDataMemberNames
    {
        [DataMemberNamesClass(typeof(ClientMessageDataMemberNames),
            isArray: false)]
        public const string
            ChatRoomMessage = "c";
    }
}
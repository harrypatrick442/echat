using Chat.DataMemberNames.Requests;
using Chat.Messages.Client.Messages;
using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class SendChatRoomMessageRequest : TicketedMessageBase
    {
        [JsonPropertyName(SendChatRoomMessageRequestDataMemberNames.ChatRoomMessage)]
        [JsonInclude]
        [DataMember(Name = SendChatRoomMessageRequestDataMemberNames.ChatRoomMessage)]
        public ClientMessage ChatRoomMessage
        {
            get;
            protected set;
        }
        public SendChatRoomMessageRequest() : base(MessageTypes.ChatRoomSendMessage)
        {

        }
    }
}

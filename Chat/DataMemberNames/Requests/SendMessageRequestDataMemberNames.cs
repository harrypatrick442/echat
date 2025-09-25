using MessageTypes.Attributes;
using Chat.DataMemberNames.Messages;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(MessageTypes.ClientMessage)]
    public class SendMessageRequestDataMemberNames : ClientMessageDataMemberNames
    {

    }
}
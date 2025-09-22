using MessageTypes.Attributes;
using Chat.DataMemberNames.Messages;

namespace Chat.DataMemberNames.Requests
{
    [MessageType(global::MessageTypes.MessageTypes.ClientMessage)]
    public class SendMessageRequestDataMemberNames : ClientMessageDataMemberNames
    {

    }
}
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class IncorrectPasswordMessage : TicketedMessageBase
    {
        public IncorrectPasswordMessage()
            : base(MessageTypes.ChatIncorrectPassword)
        {
        }
    }
}

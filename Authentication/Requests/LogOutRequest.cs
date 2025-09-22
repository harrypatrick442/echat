using System.Runtime.Serialization;
using Core.Messages.Messages;

namespace Authentication.Requests
{
    [DataContract]
    public class LogOutRequest : TicketedMessageBase
    {
        public LogOutRequest() : base(global::MessageTypes.MessageTypes.AuthenticationLogOut)
        {

        }
    }
}

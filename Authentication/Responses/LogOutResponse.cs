using Core.Messages.Responses;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Authentication.Responses
{
    [DataContract]
    public class LogOutResponse : TicketedResponseMessageBase
    {
        public LogOutResponse(long ticket) : base(ticket) { }
        protected LogOutResponse() : base(0)
        {

        }
    }
}

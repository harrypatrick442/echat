using Core.Messages.Responses;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace InterserverComs
{
    [DataContract]
    public class TestInterserverConnectionResponseMessage:TicketedResponseMessageBase
    {
        public TestInterserverConnectionResponseMessage(long ticket) : base(ticket) {

        }
        protected TestInterserverConnectionResponseMessage() : base(0) { }
    }
}

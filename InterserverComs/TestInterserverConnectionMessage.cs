using Core.Messages.Messages;
using MessageTypes.Internal;

namespace InterserverComs
{
    public class TestInterserverConnectionMessage : TicketedMessageBase
    {
        public TestInterserverConnectionMessage() : base(InterserverMessageTypes.TestInterserverConnection) { 
        
        }
    }
}
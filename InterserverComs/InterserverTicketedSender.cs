using Core.Exceptions;
using Shutdown;
using Core.Ticketing;
using Core.Messages.Messages;
using Logging;

namespace InterserverComs
{
    public static class InterserverTicketedSender
    {
        private static TicketedSender _TicketedSender;
        public static void Initialize()
        {
            if (_TicketedSender != null) throw new AlreadyInitializedException(nameof(InterserverInverseTicketedSender));
            _TicketedSender = new TicketedSender();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.TicketedSender);
        }
        public static TResponseMessage Send<TMessage, TResponseMessage>(
            TMessage message, 
            int timeoutMilliseconds,
            CancellationToken? cancellationToken,
            Action<string> send)
        where TMessage : ITicketedMessageBase where TResponseMessage : ITicketedMessageBase
        {
            return _TicketedSender.Send<TMessage, TResponseMessage>(message, timeoutMilliseconds, cancellationToken, send);
        }
        public static bool HandleMessage(TicketedMessageBase message, string rawMessage)
        {
            return _TicketedSender.HandleMessage(message, rawMessage);
        }
        public static void Dispose() {
            _TicketedSender.Dispose();
        }
    }
}
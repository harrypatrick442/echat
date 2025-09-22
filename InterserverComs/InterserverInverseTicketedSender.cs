using Core.Exceptions;
using Core.Interfaces;
using Shutdown;
using Core.Ticketing;
using Core.Messages.Messages;
using Logging;

namespace InterserverComs
{
    public static class InterserverInverseTicketedSender
    {
        private static InverseTicketedSender _InverseTicketedSender;
        public static void Initialize() {
            if (_InverseTicketedSender != null) throw new AlreadyInitializedException(nameof(InterserverInverseTicketedSender));
            _InverseTicketedSender = new InverseTicketedSender();
            ShutdownManager.Instance.Add(Dispose, ShutdownOrder.TicketedSender);
        }
        public static TResponseMessage Send<TMessage, TResponseMessage>(
            TMessage message,
            long ticketForResponse,
            int timeoutMilliseconds,
            CancellationToken? cancellationToken,
            Action<string> send)

            where TMessage : TicketedMessageBase, IInverseTicketed where TResponseMessage : IInverseTicketed
        {
            if (_InverseTicketedSender == null) throw new NotInitializedException(nameof(InterserverInverseTicketedSender));
            return _InverseTicketedSender.Send<TMessage, TResponseMessage>(message, ticketForResponse, timeoutMilliseconds, cancellationToken, send);
        }
        public static bool HandleMessage<TMessage>(TMessage message, string rawMessage)
            where TMessage : IInverseTicketed, ITypedMessage
        {
            if (_InverseTicketedSender == null) throw new NotInitializedException(nameof(InterserverInverseTicketedSender));
            return _InverseTicketedSender.HandleMessage(message, rawMessage);
        }
        public static void Dispose()
        {
            _InverseTicketedSender.Dispose();
        }
    }
}
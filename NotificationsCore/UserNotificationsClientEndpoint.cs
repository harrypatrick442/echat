using Core.Messages;
using Core.Handlers;
using Core.InterserverComs;
using JSON;
using Core.Interfaces;
using Core;
using NotificationsCore.Messages.Requests;
using Chat.Messages.Client.Responses;

namespace NotificationsCore
{
    public class UserNotificationsClientEndpoint
    {
        private IClientEndpoint _ClientEndpoint;
        private long _MyUserId { get { return _ClientEndpoint.UserId; } }
        private Action _RemoveClientMessageTypeMappings;
        public UserNotificationsClientEndpoint(
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpoint clientEndpoint)
        {
            _ClientEndpoint = clientEndpoint;
            _RemoveClientMessageTypeMappings = clientMessageTypeMappingsHandler.AddRange(new TupleList<string, DelegateHandleMessageOfType<TypeTicketedAndWholePayload>> {
                { MessageTypes.UserNotificationsClear, ClearUserNotifications},
            });
        }
        private void ClearUserNotifications(TypeTicketedAndWholePayload message)
        {
            ClearUserNotificationRequest request = Json.Deserialize<ClearUserNotificationRequest>(message.JsonString);
            bool cleared = UserNotificationsMesh.Instance.ClearUserNotification(_MyUserId, request.NotificationType, request.UpToAtInclusive);
            _ClientEndpoint.SendObject(new ClearUserNotificationResponse(cleared, request.Ticket));
        }
        public void Dispose()
        {
            _RemoveClientMessageTypeMappings();
        }
    }
}
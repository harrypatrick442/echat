using Core.Handlers;
using Logging;
using InterserverComs;
using MessageTypes.Internal;
using Core;
using NotificationsCore.Messages.Requests;
using Chat.Messages.Client.Responses;
using JSON;
using NotificationsCore.Messages.Messages;

namespace NotificationsCore
{
    public partial class UserNotificationsMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    { InterserverMessageTypes.UserNotificationsClear , HandleClearUserNotification},
                    { InterserverMessageTypes.UserNotificationsGetUserNotifications , HandleGetUserNotifications},
                    { InterserverMessageTypes.UserNotificationsSetUserNotificationHasAt , HandleSetUserNotification}
                }
            );
        }

        private void HandleClearUserNotification(InterserverMessageEventArgs e)
        {
            try
            {
                ClearUserNotificationRequest request = e.Deserialize<ClearUserNotificationRequest>();
                bool cleared = ClearUserNotification_Here(request.UserId, request.NotificationType, request.UpToAtInclusive);
                e.EndpointFrom.SendJSONString(Json.Serialize(new ClearUserNotificationResponse(cleared, request.Ticket)));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleGetUserNotifications(InterserverMessageEventArgs e)
        {
            try
            {
                GetUserNotificationsRequest request = e.Deserialize<GetUserNotificationsRequest>();
                UserNotifications userNotifications = GetUserNotifications_Here(request.UserId);
                e.EndpointFrom.SendJSONString(Json.Serialize(new GetUserNotificationsResponse(userNotifications, request.Ticket)));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleSetUserNotification(InterserverMessageEventArgs e)
        {
            try
            {
                SetUserNotificationHasAtRequest request = e.Deserialize<SetUserNotificationHasAtRequest>();
                SetUserNotificationHasAt_Here(request.UserId, request.NotificationType, request.At);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}